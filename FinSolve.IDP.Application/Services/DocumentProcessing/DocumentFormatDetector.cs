using FinSolve.IDP.Domain.Enums;

namespace FinSolve.IDP.Application.Services.DocumentProcessing
{
    public class DocumentFormatDetector
    {
        private static readonly Dictionary<string, DocumentType> ExtensionMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".txt", DocumentType.Txt },
            { ".csv", DocumentType.Csv },
            { ".pdf", DocumentType.Pdf },
            { ".docx", DocumentType.Docx },
            { ".json", DocumentType.Json },
        };

        public DocumentType Detect(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return DocumentType.Unknown;

            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(extension))
                return DocumentType.Unknown;

            return ExtensionMap.TryGetValue(extension, out var type)
                ? type
                : DocumentType.Unknown;
        }
    }
}