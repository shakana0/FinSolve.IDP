using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Domain.Readers
{
    public class DocxDocumentReader : IDocumentReader
    {
        public Task<string> ReadAsTextAsync(Stream stream)
        {
            throw new NotImplementedException(
                "Docx reading requires an external library and should be implemented in Infrastructure.");
        }
    }
}
