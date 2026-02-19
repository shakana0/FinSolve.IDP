using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Domain.ValueObjects;
using Microsoft.Azure.Functions.Worker;

namespace FinSolve.IDP.Functions;

public class FinalizationFunction
{
    private readonly IProcessingResultRepository _processingRepo;
    private readonly IDocumentStatusRepository _statusRepo;
    private readonly ILoggingAdapter _logger;

    public FinalizationFunction(
        IProcessingResultRepository processingRepo,
        IDocumentStatusRepository statusRepo,
        ILoggingAdapter logger)
    {
        _processingRepo = processingRepo;
        _statusRepo = statusRepo;
        _logger = logger;
    }

    [Function("FinalizationFunction")]
    public async Task RunAsync(
        [ServiceBusTrigger("idp-documents", "pdf-generated", Connection = "ServiceBusConnection")]
        PdfMetadataDto metadata)
    {
        _logger.LogInformation($"Finalizing document {metadata.DocumentId}");

        var documentId = new DocumentId(Guid.Parse(metadata.DocumentId));

        var result = await _processingRepo.GetAsync(documentId);
        if (result is null)
        {
            throw new InvalidOperationException($"Processing result not found for document {metadata.DocumentId}");
        }

        var status = new DocumentStatusDto
        {
            DocumentId = metadata.DocumentId,
            Status = "Completed",
            PdfBlobPath = metadata.PdfBlobPath,
            CompletedUtc = DateTime.UtcNow,
            PrimaryCategory = result.PrimaryCategory,
            Items = result.Items,
            Summary = result.Summary
        };

        await _statusRepo.SaveAsync(status);

        _logger.LogInformation($"Document {metadata.DocumentId} finalized and saved to Cosmos");
    }
}
