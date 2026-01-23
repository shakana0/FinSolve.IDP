using FinSolve.IDP.Application.DTOs;

namespace FinSolve.IDP.Application.Interfaces;

public interface IDocumentStatusRepository
{
    Task SaveAsync(DocumentStatusDto status);
}
