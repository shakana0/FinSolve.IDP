using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Application.Services.Pdf
{
    public class PdfGenerationService
    {
        private readonly IPdfGenerator _pdfGenerator;

        public PdfGenerationService(IPdfGenerator pdfGenerator)
        {
            _pdfGenerator = pdfGenerator;
        }

        public async Task<byte[]> GenerateAsync(Summary summary)
        {
            if (summary == null)
                throw new ArgumentNullException(nameof(summary));

            var pdfBytes = await _pdfGenerator.GenerateAsync(summary);

            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new InvalidOperationException("PDF generation returned empty content.");

            return pdfBytes;
        }
    }
}
