using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.ValueObjects;
using Microsoft.Azure.Functions.Worker;

namespace FinSolve.IDP.Functions.SummaryGenerator;

public class SummaryGeneratorFunction
{
    private readonly ILoggingAdapter _logger;
    private readonly IProcessingResultRepository _resultRepository;
    private readonly IMessagePublisher _publisher;

    public SummaryGeneratorFunction(
        ILoggingAdapter logger,
        IProcessingResultRepository resultRepository,
        IMessagePublisher publisher)
    {
        _logger = logger;
        _resultRepository = resultRepository;
        _publisher = publisher;
    }

    [Function("SummaryGenerator")]
    public async Task RunAsync(
        [ServiceBusTrigger("idp-documents", "processing-completed", Connection = "ServiceBusConnection")]
        string message)
    {
        _logger.LogInformation("SummaryGeneratorFunction triggered");

        var metadata = DocumentMetadataDto.FromJson(message);
        if (!metadata.IsValid())
        {
            throw new ArgumentException("Invalid metadata received");
        }
        _logger.LogInformation($"Generating summary for document: {metadata.DocumentId}");

        var documentId = new DocumentId(Guid.Parse(metadata.DocumentId!));

        var result = await _resultRepository.GetAsync(documentId);
        if (result is null)
        {
            _logger.LogError($"No processing result found for {documentId}");
            return;
        }

        var updated = GenerateSummary(result);
        _logger.LogInformation("Summary generated");

        await _resultRepository.SaveAsync(updated);
        _logger.LogInformation("Updated processing result saved");

        await _publisher.PublishAsync("idp-documents", metadata, "SummaryCreated");
        _logger.LogInformation("Published event to PdfGenerator");

        _logger.LogInformation("SummaryGenerator completed successfully");
    }

    private ProcessingResult GenerateSummary(ProcessingResult result)
    {
        var summary = $"Document categorized as {result.PrimaryCategory}. " +
                      $"Contains {result.Items.Count} extracted items.";

        return new ProcessingResult(
            result.DocumentId,
            result.PrimaryCategory,
            result.Items,
            summary
        );
    }
}
