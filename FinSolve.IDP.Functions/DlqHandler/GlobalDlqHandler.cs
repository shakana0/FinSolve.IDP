using Microsoft.Azure.Functions.Worker;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Application.DTOs;

namespace FinSolve.IDP.Functions.DlqHandlers;

public class GlobalDlqHandler
{
    private readonly ILoggingAdapter _logger;
    private readonly IDlqRepository _dlqRepository;

    public GlobalDlqHandler(
        ILoggingAdapter logger,
        IDlqRepository dlqRepository)
    {
        _logger = logger;
        _dlqRepository = dlqRepository;
    }

    [Function("MetadataDlqHandler")]
    public async Task RunMetadataAsync(
        [ServiceBusTrigger("idp-documents/Subscriptions/metadata-validated/$DeadLetterQueue", Connection = "ServiceBusConnection")]
        string message, string deadLetterReason, string deadLetterErrorDescription)
    {
        await ProcessDlqMessage("metadata-validated", message, deadLetterReason, deadLetterErrorDescription);
    }

    [Function("HeavyProcessingDlqHandler")]
    public async Task RunHeavyAsync(
        [ServiceBusTrigger("idp-documents/Subscriptions/processing-completed/$DeadLetterQueue", Connection = "ServiceBusConnection")]
        string message, string deadLetterReason, string deadLetterErrorDescription)
    {
        await ProcessDlqMessage("processing-completed", message, deadLetterReason, deadLetterErrorDescription);
    }

    [Function("SummaryDlqHandler")]
    public async Task RunSummaryAsync(
        [ServiceBusTrigger("idp-documents/Subscriptions/summary-created/$DeadLetterQueue", Connection = "ServiceBusConnection")]
        string message, string deadLetterReason, string deadLetterErrorDescription)
    {
        await ProcessDlqMessage("summary-created", message, deadLetterReason, deadLetterErrorDescription);
    }

    [Function("PdfDlqHandler")]
    public async Task RunPdfAsync(
        [ServiceBusTrigger("idp-documents/Subscriptions/pdf-generated/$DeadLetterQueue", Connection = "ServiceBusConnection")]
        string message, string deadLetterReason, string deadLetterErrorDescription)
    {
        await ProcessDlqMessage("pdf-generated", message, deadLetterReason, deadLetterErrorDescription);
    }

    private async Task ProcessDlqMessage(string source, string message, string reason, string description)
    {
        // Try extracting DocumentId for correlation/traceability
        string? extractedDocId = null;
        try
        {
            // We use the DTO's built-in FromJson to peek at the content
            var originalMetadata = DocumentMetadataDto.FromJson(message);
            extractedDocId = originalMetadata.DocumentId;
        }
        catch
        {
            // If it fails (e.g. the message is corrupt), we log it but continue saving
            _logger.LogWarning($"Could not extract DocumentId from DLQ message body from {source}. Body might not be valid JSON.");
        }

        _logger.LogError($"Message ended up in DLQ from source: {source}. DocumentId: {extractedDocId ?? "Unknown"}");
        _logger.LogInformation($"Reason: {reason}, Description: {description}");

        var dto = new DeadLetterMessageDto
        {
            Id = Guid.NewGuid().ToString(),
            DocumentId = extractedDocId, // This links the error to the original document in CosmosDB
            OriginalQueue = $"idp-documents/Subscriptions/{source}",
            Body = message,
            ErrorReason = reason,
            ErrorDescription = description,
            FailedAtUtc = DateTime.UtcNow
        };

        await _dlqRepository.SaveAsync(dto);
        _logger.LogInformation($"Dead letter message for Document {extractedDocId ?? "Unknown"} from {source} persisted to storage");
    }
}