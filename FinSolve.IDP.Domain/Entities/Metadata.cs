using FinSolve.IDP.Domain.Enums;

namespace FinSolve.IDP.Domain.Entities
{
    public class Metadata
    {
        public string Title { get; }
        public string Author { get; }
        public DateTime? CreatedDate { get; }
        public DocumentType? Type { get; }
        public Dictionary<string, string> CustomFields { get; }

        public Metadata(string title, string author, DateTime? createdDate, Dictionary<string, string> customFields, DocumentType type)
        {
            Title = title;
            Author = author;
            CreatedDate = createdDate;
            Type = type;
            CustomFields = customFields ?? new();
        }
    }

}
