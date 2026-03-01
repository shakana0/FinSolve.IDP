using System.Text.Json;
using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Application.Services.DocumentMetadata;
using FinSolve.IDP.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using FinSolve.IDP.Domain.ValueObjects;

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
    public async Task RunAsync([EventGridTrigger] JsonElement eventGridEvent)
    {
        _logger.LogInformation("MetadataValidationFunction triggered via Event Grid");

        string? blobPath = ExtractBlobPath(eventGridEvent);

        if (string.IsNullOrEmpty(blobPath))
        {
            _logger.LogError("Could not extract blobPath from EventGridSchema");
            return;
        }

        blobPath = Uri.UnescapeDataString(blobPath);
        _logger.LogInformation($"Attempting to download blob from path: {blobPath}");

        var fileName = Path.GetFileName(blobPath);
        using Stream fileStream = await _blobStorage.DownloadStreamAsync(blobPath);

        // 1. Extract metadata
        var domainMetadata = await _metadataService.ExtractAsync(fileName, fileStream);

        // 2. Create DTO
        var metadata = new DocumentMetadataDto
        {
            DocumentId = Guid.NewGuid().ToString(),
            BlobPath = blobPath,
            FileName = fileName,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = "System.EventGrid",
            ContentType = domainMetadata.Type.ToString()
        };

        if (!metadata.IsValid())
        {
            _logger.LogWarning($"Invalid metadata generated for file: {fileName}");
            throw new Exception("Invalid metadata");
        }


        // 3. Generate Hash-object
        var hashValue = metadata.GenerateHash();
        var hash = new Hash(hashValue);

        // 4. Idempotens controle
        var exists = await _hashRepository.ExistsAsync(hash);

        if (exists)
        {
            _logger.LogWarning($"Duplicate document detected (Hash: {hash.Value}), skipping.");
            return;
        }

        // 5. Save
        await _hashRepository.SaveAsync(metadata.DocumentId!, hash);

        _logger.LogInformation("Hash saved for idempotency");

        await _publisher.PublishAsync("idp-documents", metadata, "MetadataValidated");
        _logger.LogInformation("Published event to Service Bus");
    }

    private string? ExtractBlobPath(JsonElement eventGridEvent)
    {
        string? rawPath = null;

        if (eventGridEvent.TryGetProperty("data", out var dataProp) &&
            dataProp.TryGetProperty("url", out var urlProp))
        {
            rawPath = urlProp.GetString();
        }

        else if (eventGridEvent.TryGetProperty("subject", out var subjectProp))
        {
            rawPath = subjectProp.GetString();
        }

        if (string.IsNullOrEmpty(rawPath)) return null;

        if (rawPath.StartsWith("http"))
        {
            var uri = new Uri(rawPath);
            return uri.AbsolutePath.TrimStart('/');
        }

        return rawPath;
    }
}