using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using FinSolve.IDP.Application.Interfaces;

namespace FinSolve.IDP.Infrastructure.Messaging
{
    public class ServiceBusMessagePublisher : IMessagePublisher
    {
        private readonly ServiceBusClient _client;

        public ServiceBusMessagePublisher(ServiceBusClient client)
        {
            _client = client;
        }

        public async Task PublishAsync(string topicName, object payload)
        {
            var sender = _client.CreateSender(topicName);

            var json = JsonSerializer.Serialize(payload);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(message);
        }
    }
}
