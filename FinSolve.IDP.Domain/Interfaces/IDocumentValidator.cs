using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Domain.Interfaces
{
    public interface IDocumentValidator
    {
        ValidationResult Validate(Metadata metadata, string content);
    }
}
