using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using Microsoft.Azure.Cosmos;

namespace FinSolve.IDP.Infrastructure.Dependencies.Cosmos;

public class DocumentStatusRepository : IDocumentStatusRepository
{
    private readonly Container _container;

    public DocumentStatusRepository(Container container)
    {
        _container = container;
    }

    public async Task SaveAsync(DocumentStatusDto status)
    {
        await _container.UpsertItemAsync(status, new PartitionKey(status.DocumentId));
    }
}
