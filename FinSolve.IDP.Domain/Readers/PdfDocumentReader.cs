using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Domain.Readers
{
    public class PdfDocumentReader : IDocumentReader
    {
        public Task<string> ReadAsTextAsync(Stream stream)
        {
            throw new NotImplementedException(
                "PDF reading requires an external library and should be implemented in Infrastructure.");
        }
    }
}
