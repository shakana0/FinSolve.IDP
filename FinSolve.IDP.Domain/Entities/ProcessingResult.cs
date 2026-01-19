using FinSolve.IDP.Domain.ValueObjects;
namespace FinSolve.IDP.Domain.Entities
{
    public class ProcessingResult
    {
        public DocumentId DocumentId { get; }
        public string Summary { get; }
        public Dictionary<string, object> MappedData { get; }

        public ProcessingResult(DocumentId documentId, string summary, Dictionary<string, object> mappedData)
        {
            DocumentId = documentId;
            Summary = summary;
            MappedData = mappedData ?? new();
        }
    }

}
