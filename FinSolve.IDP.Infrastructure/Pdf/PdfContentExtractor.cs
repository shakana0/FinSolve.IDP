using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.Enums;
using FinSolve.IDP.Domain.Interfaces;
namespace FinSolve.IDP.Infrastructure.Pdf
{
    public class PdfContentExtractor : IMetadataExtractor
    {
        public async Task<Metadata> ExtractAsync(string content)
        {
            var lines = content.Split('\n');
            var title = lines.Length > 0 ? lines[0].Trim() : "Unknown";
            return new Metadata(
                title: title,
                author: "System Extracted",
                createdDate: DateTime.UtcNow,
                customFields: new Dictionary<string, string>(),
                type: DocumentType.Unknown
            );
        }
    }
}