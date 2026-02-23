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
using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {

        // Cosmos DB
        services.AddSingleton(sp => new CosmosClient(
            config["CosmosDbAccountEndpoint"],
            new DefaultAzureCredential()));

        services.AddSingleton<IDocumentHashRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(config["Cosmos__Database"], config["Cosmos__StatusContainer"]);
            return new DocumentHashRepository(container);
        });

        services.AddSingleton<IDocumentStatusRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(config["Cosmos__Database"], config["Cosmos__StatusContainer"]);
            return new DocumentStatusRepository(container);
        });

        services.AddSingleton<IProcessingResultRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(config["Cosmos__Database"], config["Cosmos__ResultContainer"]);
            return new ProcessingResultRepository(container);
        });

        services.AddSingleton<IDlqRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(config["Cosmos__Database"], config["Cosmos__DlqContainer"]);
            return new DlqRepository(container);
        });

        // Blob Storage
        services.AddSingleton(sp =>
        {
            var accountName = config["StorageAccountName"];
            if (string.IsNullOrEmpty(accountName))
            {
                throw new InvalidOperationException("Configuration 'StorageAccountName' is missing. Check Azure App Settings.");
            }

            return new BlobServiceClient(
                new Uri($"https://{accountName}.blob.core.windows.net"),
                new DefaultAzureCredential());
        });

        services.AddSingleton(sp =>
      {
          var service = sp.GetRequiredService<BlobServiceClient>();
          var containerName = config["BlobStorage:ContainerName"] ?? "documents";
          return service.GetBlobContainerClient(containerName);
      });

        // Service Bus
        services.AddSingleton(sp => new ServiceBusClient(
          config["ServiceBusConnection__fullyQualifiedNamespace"],
          new DefaultAzureCredential()));

        // Blob Storage
        services.AddSingleton<IBlobStorage, AzureBlobStorage>();

        // Messaging
        services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();

        // PDF Generation
        services.AddScoped<IMetadataExtractor, PdfContentExtractor>();
        services.AddSingleton<IReportGenerator, QuestPdfGenerator>();

        // Security & Telemetry
        services.AddSingleton<IKeyVaultSecretProvider, KeyVaultSecretProvider>();
        services.AddSingleton<ILoggingAdapter, ApplicationInsightsLogger>();

        return services;
    }
}