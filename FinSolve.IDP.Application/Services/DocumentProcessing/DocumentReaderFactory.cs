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
            _logger.LogInformation($"Factory received {_readers.Count()} total readers from DI. Attempting to find match for: {type}");
            _logger.LogInformation($"Available types: {string.Join(", ", _readers.Select(r => r.GetType().Name))}");

            var reader = type switch
            {
                DocumentType.Txt => _readers.OfType<TxtDocumentReader>().FirstOrDefault() as IDocumentReader,
                DocumentType.Csv => _readers.OfType<CsvDocumentReader>().FirstOrDefault() as IDocumentReader,
                DocumentType.Pdf => _readers.OfType<PdfDocumentReader>().FirstOrDefault() as IDocumentReader,
                DocumentType.Docx => _readers.OfType<DocxDocumentReader>().FirstOrDefault() as IDocumentReader,

                DocumentType.Json => throw new UnsupportedFormatException("JSON reader is not yet implemented"),
                DocumentType.Unknown => throw new UnsupportedFormatException("Cannot process document of unknown type"),
                _ => throw new UnsupportedFormatException($"No reader defined for {type}")
            };

            if (reader == null)
            {
                var errorMsg = $"No reader registered in DI for {type}. Ensure it is added in DependencyInjection.cs";
                _logger.LogError(errorMsg);
                throw new UnsupportedFormatException(errorMsg);
            }

            return reader!;
        }
    }
}
