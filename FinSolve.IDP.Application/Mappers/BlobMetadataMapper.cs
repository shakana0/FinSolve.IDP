using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Application.Mappers
{
    public static class BlobMetadataMapper
    {
        public static IDictionary<string, string> FromProcessingResult(ProcessingResult result)
        {
            return new Dictionary<string, string>
            {
                { "documentId", result.DocumentId.Value.ToString() },
                { "primaryCategory", result.PrimaryCategory },
                { "itemCount", result.Items.Count.ToString() },
                { "summaryPreview", CreateSummaryPreview(result.Summary) },
                { "createdUtc", DateTime.UtcNow.ToString("o") }
            };
        }

        private static string CreateSummaryPreview(string summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
                return string.Empty;

            // Blob metadata should be short
            return summary.Length <= 200
                ? summary
                : summary.Substring(0, 200);
        }
    }
}
