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
            try
            {
                var response = await _container.ReadItemAsync<DocumentHashCosmosDto>(
                    id: hash.Value,
                    partitionKey: new PartitionKey(hash.Value)
                );

                return response.Resource != null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task SaveAsync(string documentId, Hash hash)
        {
            var dto = new DocumentHashCosmosDto
            {
                id = hash.Value,
                DocumentId = documentId,
                Hash = hash.Value
            };

            await _container.UpsertItemAsync(dto, new PartitionKey(hash.Value));
        }
    }
}