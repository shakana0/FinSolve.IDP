using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Domain.Interfaces;
using Microsoft.Azure.Cosmos;
using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Infrastructure.Cosmos
{
    public class DocumentHashRepository : IDocumentHashRepository
    {
        private readonly Container _container;

        public DocumentHashRepository(Container container)
        {
            _container = container;
        }

        public async Task<bool> ExistsAsync(Hash hash)
        {
            var query = new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.id = @hash")
                .WithParameter("@hash", hash.Value);

            var iterator = _container.GetItemQueryIterator<int>(query);
            var response = await iterator.ReadNextAsync();
            return response.First() > 0;
        }

        public async Task SaveAsync(string documentId, Hash hash)
        {
            var dto = new DocumentHashCosmosDto
            {
                Id = hash.Value,
                DocumentId = documentId,
                Hash = hash.Value
            };

            await _container.UpsertItemAsync(dto, new PartitionKey(documentId));
        }
    }
}