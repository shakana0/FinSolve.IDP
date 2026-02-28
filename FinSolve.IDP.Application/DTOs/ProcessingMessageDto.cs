using System.Text.Json.Serialization;

namespace FinSolve.IDP.Application.DTOs
{
    public class ProcessingMessageDto
    {
        [JsonPropertyName("documentId")]
        public Guid DocumentId { get; set; }
        public string PrimaryCategory { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}
