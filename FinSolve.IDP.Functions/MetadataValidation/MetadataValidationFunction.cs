using Azure.Messaging;
using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Application.Services.DocumentMetadata;
using Microsoft.Azure.Functions.Worker;

namespace FinSolve.IDP.Functions.MetadataValidation;

public class MetadataValidationFunction
{
    private readonly ILoggingAdapter _logger;
    private readonly IDocumentHashRepository _hashRepository;
    private readonly IMessagePublisher _publisher;
    private readonly MetadataExtractionService _metadataService;
    private readonly IBlobStorage _blobStorage;

    public MetadataValidationFunction(
        ILoggingAdapter logger,
        IDocumentHashRepository hashRepository,
        IMessagePublisher publisher,
        MetadataExtractionService metadataService,
        IBlobStorage blobStorage)
    {
        _logger = logger;
        _hashRepository = hashRepository;
        _publisher = publisher;
        _metadataService = metadataService;
        _blobStorage = blobStorage;
    }

    [Function("MetadataValidation")]
    public async Task RunAsync([EventGridTrigger] CloudEvent cloudEvent)
    {
        _logger.LogInformation("MetadataValidationFunction triggered via Event Grid");

        // 1. Extract information from eventet
        // Subject usually looks like: /blobServices/default/containers/idp-documents/blobs/kvitto.pdf
        var blobPath = cloudEvent.Subject;
        var fileName = Path.GetFileName(blobPath);

        _logger.LogInformation($"Processing file from Event Grid: {fileName}");

        // 1. Download file as byte[]
        byte[] fileBytes = await _blobStorage.DownloadAsync(blobPath);

        // 2. Create a Stream (because MetadataExtractionService requires it)
        using var stream = new MemoryStream(fileBytes);

        // 3. Use Application Service to extract domain metadata
        // This handles FormatDetection, ReaderFactory and MetadataExtractor internally
        var domainMetadata = await _metadataService.ExtractAsync(fileName, stream);

        // 2.Create metadata-DTO
        var metadata = new DocumentMetadataDto
        {
            DocumentId = Guid.NewGuid().ToString(),
            BlobPath = blobPath,
            FileName = fileName,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = "System.EventGrid",
            ContentType = domainMetadata.Type.ToString()
        };

        // 3. Validation
        if (!metadata.IsValid())
        {
            _logger.LogWarning($"Invalid metadata generated for file: {fileName}");
            throw new Exception("Invalid metadata");
        }

        // 4. Idempotence check with Hash
        var hash = metadata.GenerateHash();
        var exists = await _hashRepository.ExistsAsync(metadata.DocumentId!);

        if (exists)
        {
            _logger.LogWarning($"Duplicate document detected (Hash: {hash}), skipping.");
            return;
        }

        // 5. Save and publish
        await _hashRepository.SaveHashAsync(metadata.DocumentId!, hash);
        _logger.LogInformation("Hash saved for idempotency");

        await _publisher.PublishAsync("idp-documents", metadata, "MetadataValidated");
        _logger.LogInformation("Published event to Service Bus topic: idp-documents with subject: MetadataValidated");

        _logger.LogInformation("MetadataValidation completed successfully");
    }
}
