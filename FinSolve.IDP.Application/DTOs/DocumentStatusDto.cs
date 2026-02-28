using System.Text.Json.Serialization;

namespace FinSolve.IDP.Application.DTOs;

public class DocumentStatusDto
{
    [JsonPropertyName("documentId")]
    public string DocumentId { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string PdfBlobPath { get; set; } = default!;
    public DateTime CompletedUtc { get; set; }
    public string PrimaryCategory { get; set; } = default!;
    public IReadOnlyList<string> Items { get; set; } = Array.Empty<string>();
    public string Summary { get; set; } = default!;
}
