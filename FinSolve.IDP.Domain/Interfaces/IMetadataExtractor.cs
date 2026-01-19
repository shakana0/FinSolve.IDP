using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Domain.Interfaces
{
    public interface IMetadataExtractor
    {
        Task<Metadata> ExtractAsync(string content);
    }
}
