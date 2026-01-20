namespace FinSolve.IDP.Application.DTOs
{
    public class ProcessingResultCosmosDto
    {
        public string id { get; set; } = default!;
        public string DocumentId { get; set; } = default!;
        public string PrimaryCategory { get; set; } = default!;
        public List<string> Items { get; set; } = new();
        public string Summary { get; set; } = default!;
        public string PartitionKey { get; set; } = default!;
        public DateTime CreatedUtc { get; set; }
    }
}
