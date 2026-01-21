using FinSolve.IDP.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace FinSolve.IDP.Infrastructure.Pdf
{
    public class PdfGenerator : IPdfGenerator
    {
        public byte[] GeneratePdf(string title, string content)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text(title).FontSize(18).Bold().AlignCenter();
                    page.Content().PaddingVertical(10).Text(content);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
