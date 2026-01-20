using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Domain.Interfaces
{
    public interface IProcessingRules
    {
        Task<ProcessingResult> ApplyAsync(Metadata metadata, string content);
    }
}
