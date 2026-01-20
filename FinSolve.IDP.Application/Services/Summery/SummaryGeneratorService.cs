using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Application.Services.Summery
{
    public class SummaryGeneratorService : ISummaryGenerator
    {
        public Task<Summary> GenerateAsync(ProcessingResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var summaryText = $"Document processed successfully. " +
                              $"Detected {result.Items.Count} key items. " +
                              $"Primary category: {result.PrimaryCategory}.";

            var summary = new Summary(summaryText);

            return Task.FromResult(summary);
        }
    }
}
