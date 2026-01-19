using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Domain.Interfaces
{
    public interface IProcessingRules
    {
        Task<ProcessingResult> ApplyAsync(Document document, string content);
    }
}
