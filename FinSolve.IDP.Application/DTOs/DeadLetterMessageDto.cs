using System.Text.Json.Serialization;

namespace FinSolve.IDP.Application.DTOs
{
    public class DeadLetterMessageDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonPropertyName("documentId")]
        public string? DocumentId { get; set; }
        public string OriginalQueue { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string ErrorReason { get; set; } = default!;
        public string ErrorDescription { get; set; } = default!;
        public DateTime FailedAtUtc { get; set; } = DateTime.UtcNow;
    }

}