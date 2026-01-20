using FinSolve.IDP.Domain.ValueObjects;
namespace FinSolve.IDP.Domain.Entities
{
    public class ProcessingResult
    {
        public DocumentId DocumentId { get; }
        public string PrimaryCategory { get; }
        public IReadOnlyList<string> Items { get; }
        public string Summary { get; }

        public ProcessingResult(
            DocumentId documentId,
            string primaryCategory,
            IEnumerable<string> items,
            string summary)
        {
            DocumentId = documentId;
            PrimaryCategory = primaryCategory;
            Items = items.ToList().AsReadOnly();
            Summary = summary;
        }
    }

}
