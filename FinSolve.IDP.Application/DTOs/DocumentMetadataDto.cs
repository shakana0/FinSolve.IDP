using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace FinSolve.IDP.Application.DTOs
{
    public class DocumentMetadataDto
    {
        [JsonPropertyName("documentId")]
        public string? DocumentId { get; set; }
        public string? FileName { get; set; }
        public string? UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? BlobPath { get; set; }
        public string? ContentType { get; set; }

        public static DocumentMetadataDto FromJson(string json)
            => JsonSerializer.Deserialize<DocumentMetadataDto>(json)
            ?? throw new JsonException("Failed to deserialize DocumentMetadataDto");

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DocumentId)
                && !string.IsNullOrWhiteSpace(FileName)
                && !string.IsNullOrWhiteSpace(BlobPath)
                && UploadedAt != default;
        }

        public string GenerateHash()
        {
            var raw = $"{FileName}-{BlobPath}";
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
        }
    }
}