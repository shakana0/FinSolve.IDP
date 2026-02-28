using System.Text.Json.Serialization;

namespace FinSolve.IDP.Application.DTOs;

public class PdfMetadataDto
{
    [JsonPropertyName("documentId")]
    public string DocumentId { get; set; } = default!;
    public string PdfBlobPath { get; set; } = default!;
}
