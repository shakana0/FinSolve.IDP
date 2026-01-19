using FinSolve.IDP.Domain.Interfaces;
using System.Text;

namespace FinSolve.IDP.Domain.Readers
{
    public class TxtDocumentReader : IDocumentReader
    {
        public async Task<string> ReadAsTextAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }
    }
}
