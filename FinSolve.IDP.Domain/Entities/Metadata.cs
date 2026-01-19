namespace FinSolve.IDP.Domain.Entities
{
    public class Metadata
    {
        public string Title { get; }
        public string Author { get; }
        public DateTime? CreatedDate { get; }
        public Dictionary<string, string> CustomFields { get; }

        public Metadata(string title, string author, DateTime? createdDate, Dictionary<string, string> customFields)
        {
            Title = title;
            Author = author;
            CreatedDate = createdDate;
            CustomFields = customFields ?? new();
        }
    }

}
