using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Application.DTOs;

namespace FinSolve.IDP.Functions.MetadataValidation;

public class MetadataValidationFunction
{
    private readonly ILoggingAdapter _logger;
    private readonly IDocumentHashRepository _hashRepository;
    private readonly IMessagePublisher _publisher;

    public MetadataValidationFunction(
        ILoggingAdapter logger,
        IDocumentHashRepository hashRepository,
        IMessagePublisher publisher)
    {
        _logger = logger;
        _hashRepository = hashRepository;
        _publisher = publisher;
    }

    [Function("MetadataValidation")]
    public async Task RunAsync(
        [ServiceBusTrigger("incoming-documents", Connection = "ServiceBusConnection")]
        string message)
    {
        _logger.LogInformation("MetadataValidationFunction triggered");

        var metadata = DocumentMetadataDto.FromJson(message);
        _logger.LogInformation($"Received document: {metadata.DocumentId}");

        if (!metadata.IsValid())
        {
            _logger.LogWarning("Invalid metadata, sending to DLQ");
            throw new System.Exception("Invalid metadata");
        }

        var hash = metadata.GenerateHash();
        var exists = await _hashRepository.ExistsAsync(metadata.DocumentId!);

        if (exists)
        {
            _logger.LogWarning("Duplicate document detected, skipping processing");
            return;
        }

        await _hashRepository.SaveHashAsync(metadata.DocumentId!, hash);
        _logger.LogInformation("Hash saved for idempotency");


        await _publisher.PublishAsync("heavy-processing", metadata);
        _logger.LogInformation("Published event to HeavyProcessing");

        _logger.LogInformation("MetadataValidation completed successfully");
    }
}
