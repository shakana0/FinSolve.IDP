using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.ValueObjects;
using Microsoft.Azure.Functions.Worker;

namespace FinSolve.IDP.Functions.HeavyProcessing;

public class HeavyProcessingFunction
{
    private readonly ILoggingAdapter _logger;
    private readonly IBlobStorage _blobStorage;
    private readonly IProcessingResultRepository _resultRepository;
    private readonly IMessagePublisher _publisher;

    public HeavyProcessingFunction(
        ILoggingAdapter logger,
        IBlobStorage blobStorage,
        IProcessingResultRepository resultRepository,
        IMessagePublisher publisher)
    {
        _logger = logger;
        _blobStorage = blobStorage;
        _resultRepository = resultRepository;
        _publisher = publisher;
    }

    [Function("HeavyProcessing")]
    public async Task RunAsync(
        [ServiceBusTrigger("idp-documents", "metadata-validated", Connection = "ServiceBusConnection")]
        string message)
    {
        _logger.LogInformation("HeavyProcessingFunction triggered");

        // 1. Deserialize metadata
        var metadata = DocumentMetadataDto.FromJson(message);
        if (!metadata.IsValid())
        {
            throw new ArgumentException("Invalid metadata received");
        }
        _logger.LogInformation($"Processing document: {metadata.DocumentId}");

        // 2. Download file from Blob Storage
        var fileBytes = await _blobStorage.DownloadAsync(metadata.BlobPath!);
        _logger.LogInformation("File downloaded from Blob Storage");

        // 3. Perform heavy processing
        var documentId = new DocumentId(Guid.Parse(metadata.DocumentId!));
        var result = ProcessDocument(documentId, fileBytes);
        _logger.LogInformation("Heavy processing completed");

        // 4. Save processing result
        await _resultRepository.SaveAsync(result);
        _logger.LogInformation("Processing result saved");

        // 5. Publish event to next step (SummaryGenerator)
        await _publisher.PublishAsync("idp-documents", metadata, "ProcessingCompleted");
        _logger.LogInformation("Published event to SummaryGenerator");

        _logger.LogInformation("HeavyProcessing completed successfully");
    }

    private ProcessingResult ProcessDocument(DocumentId documentId, byte[] fileBytes)
    {
        return new ProcessingResult(
            documentId,
            "Unknown",
            new List<string> { $"File size: {fileBytes.Length} bytes" },
            "Basic processing completed"
        );
    }
}
