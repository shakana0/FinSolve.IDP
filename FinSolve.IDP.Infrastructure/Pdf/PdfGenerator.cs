using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Domain.Entities;
using QuestPDF.Fluent;
using Document = QuestPDF.Fluent.Document;

namespace FinSolve.IDP.Infrastructure.Pdf
{
    public class QuestPdfGenerator : IPdfGenerator
    {
        public byte[] Generate(ProcessingResult result)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Document ID: {result.DocumentId.Value}");
                        col.Item().Text($"Category: {result.PrimaryCategory}");
                        col.Item().Text("Items:");
                        foreach (var item in result.Items)
                            col.Item().Text($"- {item}");
                        col.Item().Text("Summary:");
                        col.Item().Text(result.Summary);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
