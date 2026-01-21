using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using Microsoft.Azure.Cosmos;


namespace FinSolve.IDP.Infrastructure.Cosmos
{
    public class ProcessingResultRepository : IProcessingResultRepository
    {
        private readonly Container _container;

        public ProcessingResultRepository(Container container)
        {
            _container = container;
        }

        public async Task SaveAsync(ProcessingResultCosmosDto dto)
        {
            await _container.UpsertItemAsync(dto, new PartitionKey(dto.DocumentId));
        }
    }
}
