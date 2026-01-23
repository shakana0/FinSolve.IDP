using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using Microsoft.Azure.Cosmos;

namespace FinSolve.IDP.Infrastructure.Dependencies.Cosmos;

public class DocumentStatusRepository : IDocumentStatusRepository
{
    private readonly Container _container;

    public DocumentStatusRepository(CosmosClient client)
    {
        _container = client.GetContainer("idp", "document-status");
    }

    public async Task SaveAsync(DocumentStatusDto status)
    {
        await _container.UpsertItemAsync(status, new PartitionKey(status.DocumentId));
    }
}
