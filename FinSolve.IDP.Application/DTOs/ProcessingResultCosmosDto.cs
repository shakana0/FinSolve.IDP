using System.Text.Json.Serialization;

namespace FinSolve.IDP.Application.DTOs
{
    public class ProcessingResultCosmosDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;
        [JsonPropertyName("documentId")]
        public string DocumentId { get; set; } = default!;
        public string PrimaryCategory { get; set; } = default!;
        public List<string> Items { get; set; } = new();
        public string Summary { get; set; } = default!;
        public DateTime CreatedUtc { get; set; }
    }
}
