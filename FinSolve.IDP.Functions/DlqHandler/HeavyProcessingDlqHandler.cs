using Microsoft.Azure.Functions.Worker;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Application.DTOs;

namespace FinSolve.IDP.Functions.DlqHandlers;

public class HeavyProcessingDlqHandler
{
    private readonly ILoggingAdapter _logger;
    private readonly IDlqRepository _dlqRepository;

    public HeavyProcessingDlqHandler(
        ILoggingAdapter logger,
        IDlqRepository dlqRepository)
    {
        _logger = logger;
        _dlqRepository = dlqRepository;
    }

    [Function("HeavyProcessingDlqHandler")]
    public async Task RunAsync(
        [ServiceBusTrigger("heavy-processing/$DeadLetterQueue", Connection = "ServiceBusConnection")]
        string message,
        string deadLetterReason,
        string deadLetterErrorDescription)
    {
        _logger.LogError("Message ended up in DLQ for HeavyProcessing");
        _logger.LogError($"Reason: {deadLetterReason}");
        _logger.LogError($"Description: {deadLetterErrorDescription}");
        _logger.LogError($"Message body: {message}");

        var dto = new DeadLetterMessageDto
        {
            Id = Guid.NewGuid().ToString(),
            OriginalQueue = "heavy-processing",
            Body = message,
            ErrorReason = deadLetterReason,
            ErrorDescription = deadLetterErrorDescription,
            FailedAtUtc = DateTime.UtcNow
        };

        await _dlqRepository.SaveAsync(dto);

        _logger.LogInformation("DLQ message saved to Cosmos");
    }
}
