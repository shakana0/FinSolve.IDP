namespace FinSolve.IDP.Domain.Interfaces
{
    public interface IDocumentReader
    {
        Task<string> ReadAsTextAsync(Stream stream);
    }
}
