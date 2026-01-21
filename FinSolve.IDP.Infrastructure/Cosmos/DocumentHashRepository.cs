using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using Microsoft.Azure.Cosmos;

namespace FinSolve.IDP.Infrastructure.Cosmos
{
    public class DocumentHashRepository : IDocumentHashRepository
    {
        private readonly Container _container;

        public DocumentHashRepository(Container container)
        {
            _container = container;
        }

        public async Task<bool> ExistsAsync(string documentId)
        {
            try
            {
                var response = await _container.ReadItemAsync<DocumentHashCosmosDto>(
                    id: documentId,
                    partitionKey: new PartitionKey(documentId)
                );

                return response.Resource != null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task SaveHashAsync(string documentId, string hash)
        {
            var dto = new DocumentHashCosmosDto
            {
                id = documentId,
                DocumentId = documentId,
                Hash = hash
            };

            await _container.UpsertItemAsync(dto, new PartitionKey(documentId));
        }
    }
}
