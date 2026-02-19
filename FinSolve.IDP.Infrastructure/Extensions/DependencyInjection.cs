using Microsoft.Extensions.DependencyInjection;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Infrastructure.Blob;
using FinSolve.IDP.Infrastructure.Cosmos;
using FinSolve.IDP.Infrastructure.Messaging;
using FinSolve.IDP.Infrastructure.KeyVault;
using FinSolve.IDP.Infrastructure.Telemetry;
using FinSolve.IDP.Infrastructure.Dependencies.Cosmos;
using FinSolve.IDP.Infrastructure.Pdf;
using Microsoft.Azure.Cosmos;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace FinSolve.IDP.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {

        // Cosmos DB
        services.AddSingleton(sp => new CosmosClient(
            Environment.GetEnvironmentVariable("CosmosDbAccountEndpoint"),
            new DefaultAzureCredential()));

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            return client.GetContainer(config["Cosmos:Database"], config["Cosmos:Container"]);
        });

        // Blob Storage
        services.AddSingleton(sp => new BlobServiceClient(
            new Uri($"https://{config["StorageAccountName"]}.blob.core.windows.net"),
            new DefaultAzureCredential()));

        services.AddSingleton(sp =>
        {
            var service = sp.GetRequiredService<BlobServiceClient>();
            return service.GetBlobContainerClient(config["BlobStorage:ContainerName"]);
        });

        // Service Bus
        services.AddSingleton(sp => new ServiceBusClient(
            Environment.GetEnvironmentVariable("ServiceBusConnection__fullyQualifiedNamespace"),
            new DefaultAzureCredential()));

        // Blob Storage
        services.AddSingleton<IBlobStorage, AzureBlobStorage>();

        // Cosmos DB Repositories
        services.AddSingleton<IDocumentHashRepository, DocumentHashRepository>();
        services.AddSingleton<IDocumentStatusRepository, DocumentStatusRepository>();
        services.AddSingleton<IProcessingResultRepository, ProcessingResultRepository>();

        // Messaging
        services.AddSingleton<IDlqRepository, DlqRepository>();
        services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();

        // PDF Generation
        services.AddSingleton<IPdfGenerator, QuestPdfGenerator>();

        // Security & Telemetry
        services.AddSingleton<IKeyVaultSecretProvider, KeyVaultSecretProvider>();
        services.AddSingleton<ILoggingAdapter, ApplicationInsightsLogger>();

        return services;
    }
}