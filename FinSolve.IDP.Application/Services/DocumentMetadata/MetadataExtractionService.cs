using FinSolve.IDP.Application.Services.DocumentProcessing;
using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.Enums;
using FinSolve.IDP.Domain.Exceptions;
using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Application.Services.DocumentMetadata
{
    public class MetadataExtractionService
    {
        private readonly DocumentFormatDetector _formatDetector;
        private readonly DocumentReaderFactory _readerFactory;
        private readonly IMetadataExtractor _metadataExtractor;
        private readonly IDocumentValidator _validator;

        public MetadataExtractionService(
            DocumentFormatDetector formatDetector,
            DocumentReaderFactory readerFactory,
            IMetadataExtractor metadataExtractor,
            IDocumentValidator validator)
        {
            _formatDetector = formatDetector;
            _readerFactory = readerFactory;
            _metadataExtractor = metadataExtractor;
            _validator = validator;
        }

        public async Task<Metadata> ExtractAsync(string fileName, Stream stream)
        {
            var type = _formatDetector.Detect(fileName);

            if (type == DocumentType.Unknown)
                throw new UnsupportedFormatException(fileName);

            var reader = _readerFactory.GetReader(type);

            var content = await reader.ReadAsTextAsync(stream);

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidDocumentException("Document content is empty.");

            var extracted = await _metadataExtractor.ExtractAsync(content);

            if (extracted == null)
                throw new MetadataExtractionException("Failed to extract metadata.");

            var validation = _validator.Validate(extracted, content);

            if (!validation.IsValid)
                throw new InvalidDocumentException(string.Join("; ", validation.Errors));

            return new Metadata(
              extracted.Title,
              extracted.Author,
              extracted.CreatedDate,
              extracted.CustomFields,
              type                  
          );
        }
    }
}
