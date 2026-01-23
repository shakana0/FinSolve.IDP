using System.Text.Json;

namespace FinSolve.IDP.Application.DTOs 
{
    public class DocumentMetadataDto
    {
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
            var raw = $"{DocumentId}-{FileName}-{UploadedAt:o}";
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw)));
        }
    }
}

