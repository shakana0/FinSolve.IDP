using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Application.Interfaces;
using Microsoft.Azure.Cosmos;

namespace FinSolve.IDP.Infrastructure.Messaging {
    public class DlqRepository : IDlqRepository
    {
        private readonly Container _container;

        public DlqRepository(CosmosClient client)
        {
            _container = client.GetContainer("idp", "dlq");
        }

        public async Task SaveAsync(DeadLetterMessageDto message)
        {
            await _container.UpsertItemAsync(message, new PartitionKey(message.OriginalQueue));
        }
    }
}

