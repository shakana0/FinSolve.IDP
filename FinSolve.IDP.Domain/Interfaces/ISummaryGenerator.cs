using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Domain.Interfaces
{
    public interface ISummaryGenerator
    {
        Task<Summary> GenerateAsync(ProcessingResult result);
    }
}
