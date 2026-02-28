using System.Text.Json.Serialization;

namespace FinSolve.IDP.Application.DTOs
{
    public class DocumentHashCosmosDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;
        [JsonPropertyName("documentId")]
        public string DocumentId { get; set; } = default!;
        public string Hash { get; set; } = default!;
    }
}
