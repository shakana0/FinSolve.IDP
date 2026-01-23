using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.ValueObjects;
using Microsoft.Azure.Functions.Worker;

namespace FinSolve.IDP.Functions.PdfGenerator;

public class PdfGeneratorFunction
{
    private readonly ILoggingAdapter _logger;
    private readonly IProcessingResultRepository _resultRepository;
    private readonly IBlobStorage _blobStorage;
    private readonly IMessagePublisher _publisher;

    public PdfGeneratorFunction(
        ILoggingAdapter logger,
        IProcessingResultRepository resultRepository,
        IBlobStorage blobStorage,
        IMessagePublisher publisher)
    {
        _logger = logger;
        _resultRepository = resultRepository;
        _blobStorage = blobStorage;
        _publisher = publisher;
    }

    [Function("PdfGenerator")]
    public async Task RunAsync(
        [ServiceBusTrigger("pdf-generator", Connection = "ServiceBusConnection")]
        string message)
    {
        _logger.LogInformation("PdfGeneratorFunction triggered");

        var metadata = DocumentMetadataDto.FromJson(message);
        if (!metadata.IsValid())
        {
            throw new ArgumentException("Invalid metadata received");
        }
        _logger.LogInformation($"Generating PDF for document: {metadata.DocumentId}");

        var documentId = new DocumentId(Guid.Parse(metadata.DocumentId!));

        var result = await _resultRepository.GetAsync(documentId);
        if (result is null)
        {
            _logger.LogError($"No processing result found for {documentId}");
            return;
        }

        var pdfBytes = GeneratePdf(result);
        _logger.LogInformation("PDF generated");

        var blobName = $"{documentId.Value}.pdf";
        var pdfBlobPath = await _blobStorage.UploadAsync(blobName, pdfBytes, new Dictionary<string, string>
        {
            { "DocumentId", documentId.Value.ToString() },
            { "PrimaryCategory", result.PrimaryCategory }
        });

        _logger.LogInformation("PDF uploaded to Blob Storage");

        await _publisher.PublishAsync("finalization", new PdfMetadataDto
        {
            DocumentId = result.DocumentId.Value.ToString(),
            PdfBlobPath = pdfBlobPath
        });

        _logger.LogInformation("Published event to Finalization");
        _logger.LogInformation("PdfGenerator completed successfully");
    }

    private byte[] GeneratePdf(ProcessingResult result)
    {
        var content = $@"
            Document ID: {result.DocumentId.Value}
            Category: {result.PrimaryCategory}
            Items:
            {string.Join("\n", result.Items)}
            Summary:
            {result.Summary}
        ";

        return System.Text.Encoding.UTF8.GetBytes(content);
    }
}
