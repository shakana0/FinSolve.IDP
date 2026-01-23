
using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Application.Interfaces
{
    public interface IProcessingResultRepository
    {
        Task<ProcessingResult?> GetAsync(DocumentId documentId);
        Task SaveAsync(ProcessingResult result);
    }
}
