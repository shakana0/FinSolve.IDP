using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Domain.Interfaces
{
    public interface IDocumentHashRepository
    {
        Task<bool> ExistsAsync(Hash hash);
        Task SaveAsync(string documentId, Hash hash);
    }
}