namespace FinSolve.IDP.Application.Interfaces
{
    public interface IDocumentHashRepository
    {
        Task<bool> ExistsAsync(string documentId);
        Task SaveHashAsync(string documentId, string hash);
    }
}
