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
            return type switch
            {
                DocumentType.Txt => _readers.OfType<TxtDocumentReader>().First(),
                DocumentType.Csv => _readers.OfType<CsvDocumentReader>().First(),
                DocumentType.Pdf => _readers.OfType<PdfDocumentReader>().First(),
                DocumentType.Docx => _readers.OfType<DocxDocumentReader>().First(),
                _ => throw new UnsupportedFormatException(type.ToString())
            };
        }
    }
}
