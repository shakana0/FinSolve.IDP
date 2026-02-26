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
        var sbNamespace = config["ServiceBusConnection:fullyQualifiedNamespace"]
            ?? config["ServiceBusConnection__fullyQualifiedNamespace"]
            ?? throw new InvalidOperationException("DEBUG: Can not find ServiceBusConnection in App Settings. Check spelling!");

        var storageAccount = config["StorageAccountName"]
            ?? throw new InvalidOperationException("DEBUG: Can not find StorageAccountName.");

        var cosmosEndpoint = config["CosmosDbAccountEndpoint"]
            ?? throw new InvalidOperationException("DEBUG: Can not find CosmosDbAccountEndpoint.");

        var cosmosDb = config["Cosmos__Database"] ?? throw new InvalidOperationException("Missing 'Cosmos__Database'");

        services.AddSingleton(sp => new CosmosClient(cosmosEndpoint, new DefaultAzureCredential()));

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

        services.AddSingleton(sp => new BlobServiceClient(
            new Uri($"https://{storageAccount}.blob.core.windows.net"),
            new DefaultAzureCredential()));

        services.AddSingleton(sp =>
        {
            var service = sp.GetRequiredService<BlobServiceClient>();
            var containerName = config["BlobStorage__ContainerName"] ?? "documents";
            return service.GetBlobContainerClient(containerName);
        });

        services.AddSingleton(sp => new ServiceBusClient(sbNamespace, new DefaultAzureCredential()));

        services.AddSingleton<IBlobStorage, AzureBlobStorage>();
        services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();
        services.AddScoped<IMetadataExtractor, PdfContentExtractor>();
        services.AddSingleton<IReportGenerator, QuestPdfGenerator>();
        services.AddSingleton<IKeyVaultSecretProvider, KeyVaultSecretProvider>();
        services.AddSingleton<ILoggingAdapter, ApplicationInsightsLogger>();

        return services;
    }
}