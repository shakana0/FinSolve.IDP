using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Domain.Interfaces
{
    public interface IPdfGenerator
    {
        Task<byte[]> GenerateAsync(Summary summary);
    }
}
