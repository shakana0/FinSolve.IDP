using FinSolve.IDP.Application.DTOs;

namespace FinSolve.IDP.Application.Interfaces
{
    public interface IDlqRepository
    {
        Task SaveAsync(DeadLetterMessageDto message);
    }

}
