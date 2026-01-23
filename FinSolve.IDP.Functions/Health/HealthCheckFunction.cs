using System.Net;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FinSolve.IDP.Application.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FinSolve.IDP.Functions.Health
{
    public class HealthCheckFunction
    {
        private readonly CosmosClient _cosmos;
        private readonly BlobServiceClient _blob;
        private readonly ServiceBusClient _serviceBus;
        private readonly IKeyVaultSecretProvider _keyVault;

        public HealthCheckFunction(
            CosmosClient cosmos,
            BlobServiceClient blob,
            ServiceBusClient serviceBus,
            IKeyVaultSecretProvider keyVault)
        {
            _cosmos = cosmos;
            _blob = blob;
            _serviceBus = serviceBus;
            _keyVault = keyVault;
        }

        [Function("HealthCheck")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
        {
            var result = new Dictionary<string, string>();

            // Cosmos DB
            try
            {
                await _cosmos.ReadAccountAsync();
                result["cosmos"] = "Healthy";
            }
            catch
            {
                result["cosmos"] = "Unhealthy";
            }

            // Blob Storage
            try
            {
                await foreach (var _ in _blob.GetBlobContainersAsync().Take(1))
                {
                    break;
                }
                result["blob"] = "Healthy";
            }
            catch
            {
                result["blob"] = "Unhealthy";
            }

            // Service Bus
            try
            {
                var sender = _serviceBus.CreateSender("health-check");
                result["serviceBus"] = "Healthy";
            }
            catch
            {
                result["serviceBus"] = "Unhealthy";
            }

            // Key Vault
            try
            {
                await _keyVault.GetSecretAsync("health-check-secret");
                result["keyVault"] = "Healthy";
            }
            catch
            {
                result["keyVault"] = "Unhealthy";
            }

            // Build response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
    }

}

