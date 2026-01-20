using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.Enums;
using FinSolve.IDP.Domain.Exceptions;
using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Application.Services.DocumentProcessing
{
    public class DocumentProcessingService
    {
        private readonly DocumentFormatDetector _formatDetector;
        private readonly DocumentReaderFactory _readerFactory;
        private readonly IProcessingRules _processingRules;

        public DocumentProcessingService(
            DocumentFormatDetector formatDetector,
            DocumentReaderFactory readerFactory,
            IProcessingRules processingRules)
        {
            _formatDetector = formatDetector;
            _readerFactory = readerFactory;
            _processingRules = processingRules;
        }

        public async Task<ProcessingResult> ProcessAsync(string fileName, Stream stream, Metadata metadata)
        {
            var type = _formatDetector.Detect(fileName);

            if (type == DocumentType.Unknown)
                throw new UnsupportedFormatException(fileName);

            var reader = _readerFactory.GetReader(type);

            var content = await reader.ReadAsTextAsync(stream);

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidDocumentException("Document content is empty.");

            var result = await _processingRules.ApplyAsync(metadata, content);

            if (result == null)
                throw new InvalidOperationException("Processing rules returned null.");

            return result;
        }
    }
}
