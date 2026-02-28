using FinSolve.IDP.Domain.Enums;
using FinSolve.IDP.Domain.Interfaces;
using FinSolve.IDP.Domain.Readers;
using FinSolve.IDP.Domain.Exceptions;

namespace FinSolve.IDP.Application.Services.DocumentProcessing
{
    public class DocumentReaderFactory
    {
        private readonly IEnumerable<IDocumentReader> _readers;

        public DocumentReaderFactory(IEnumerable<IDocumentReader> readers)
        {
            _readers = readers;
        }

        public IDocumentReader GetReader(DocumentType type)
        {
            var reader = type switch
            {
                // We cast to (IDocumentReader) to help the compiler find the best type for the switch
                DocumentType.Txt => (IDocumentReader?)_readers.OfType<TxtDocumentReader>().FirstOrDefault(),
                DocumentType.Csv => _readers.OfType<CsvDocumentReader>().FirstOrDefault(),
                DocumentType.Pdf => _readers.OfType<PdfDocumentReader>().FirstOrDefault(),
                DocumentType.Docx => _readers.OfType<DocxDocumentReader>().FirstOrDefault(),

                DocumentType.Json => throw new UnsupportedFormatException("JSON reader is not yet implemented"),
                DocumentType.Unknown => throw new UnsupportedFormatException("Cannot process document of unknown type"),

                _ => throw new UnsupportedFormatException($"No reader defined for {type}")
            };
            // If reader is null, it means the dependency was not registered in DI
            return reader ?? throw new UnsupportedFormatException($"No reader registered in DI for {type}");
        }
    }
}
