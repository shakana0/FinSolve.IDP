using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.Interfaces;
using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Application.Services.DocumentMetadata
{
    public class DocumentValidator : IDocumentValidator
    {
        public ValidationResult Validate(Metadata metadata, string content)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(content))
                errors.Add("Document content is empty.");

            if (string.IsNullOrWhiteSpace(metadata.Title))
                errors.Add("Metadata is missing required field: Title.");

            if (string.IsNullOrWhiteSpace(metadata.Author))
                errors.Add("Metadata is missing required field: Author.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
