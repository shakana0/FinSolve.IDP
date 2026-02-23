using FinSolve.IDP.Application.Interfaces; // Innehåller IReportGenerator
using FinSolve.IDP.Domain.Interfaces;      // Innehåller IPdfGenerator
using FinSolve.IDP.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure; // <--- FIXAR CS0246 (IDocument)
using Document = QuestPDF.Fluent.Document;

namespace FinSolve.IDP.Infrastructure.Pdf
{
    public class QuestPdfGenerator : IPdfGenerator, IReportGenerator
    {
        public byte[] Generate(ProcessingResult result)
        {
            var document = CreateDocument(
                result.DocumentId.Value.ToString(),
                result.PrimaryCategory,
                result.Items,
                result.Summary);

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateAsync(Summary summary)
        {
            var document = CreateDocument(
                "N/A",
                "General Summary",
                new List<string>(),
                summary.Text);

            return await Task.FromResult(document.GeneratePdf());
        }

        private IDocument CreateDocument(string id, string category, IEnumerable<string> items, string summaryText)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Document ID: {id}");
                        col.Item().Text($"Category: {category}");
                        col.Item().Text("Items:");
                        foreach (var item in items)
                            col.Item().Text($"- {item}");
                        col.Item().Text("Summary:");
                        col.Item().Text(summaryText);
                    });
                });
            });
        }
    }
}