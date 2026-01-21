using FinSolve.IDP.Application.DTOs;

namespace FinSolve.IDP.Application.Interfaces
{
    public interface IProcessingResultRepository
    {
        Task SaveAsync(ProcessingResultCosmosDto dto);
    }
}
