using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.ValueObjects;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace FinSolve.IDP.Infrastructure.Cosmos
{
    public class ProcessingResultRepository : IProcessingResultRepository
    {
        private readonly Container _container;

        public ProcessingResultRepository(Container container)
        {
            _container = container;
        }

        public async Task SaveAsync(ProcessingResult result)
        {
            var documentIdString = result.DocumentId.Value.ToString();

            var dto = new ProcessingResultCosmosDto
            {
                id = documentIdString,
                DocumentId = documentIdString,
                PrimaryCategory = result.PrimaryCategory,
                Items = result.Items.ToList(),
                Summary = result.Summary,
                CreatedUtc = DateTime.UtcNow
            };

            await _container.UpsertItemAsync(dto, new PartitionKey(documentIdString));
        }

        public async Task<ProcessingResult?> GetAsync(DocumentId documentId)
        {
            try
            {
                var response = await _container.ReadItemAsync<ProcessingResultCosmosDto>(
                    documentId.Value.ToString(),
                    new PartitionKey(documentId.Value.ToString()));

                var dto = response.Resource;

                return new ProcessingResult(
                    new DocumentId(Guid.Parse(dto.DocumentId)),
                    dto.PrimaryCategory,
                    dto.Items,
                    dto.Summary
                );
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

    }
}
