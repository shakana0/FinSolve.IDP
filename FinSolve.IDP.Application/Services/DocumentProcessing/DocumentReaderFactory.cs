using FinSolve.IDP.Domain.Enums;
using FinSolve.IDP.Domain.Interfaces;
using FinSolve.IDP.Domain.Readers;
using FinSolve.IDP.Domain.Exceptions;
using FinSolve.IDP.Application.Interfaces;

namespace FinSolve.IDP.Application.Services.DocumentProcessing
{
    public class DocumentReaderFactory
    {
        private readonly IEnumerable<IDocumentReader> _readers;
        private readonly ILoggingAdapter _logger;


        public DocumentReaderFactory(IEnumerable<IDocumentReader> readers, ILoggingAdapter logger)
        {
            _readers = readers;
            _logger = logger;
        }

        public IDocumentReader GetReader(DocumentType type)
        {
            _logger.LogInformation($"Factory attempting to find reader for: {type}. Available readers: {string.Join(", ", _readers.Select(r => r.GetType().Name))}");

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
