using FinSolve.IDP.Domain.Interfaces;
using System.Text;

namespace FinSolve.IDP.Domain.Readers
{
    public class CsvDocumentReader : IDocumentReader
    {
        public async Task<string> ReadAsTextAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            return content.Replace("\r\n", "\n");
        }
    }
}
