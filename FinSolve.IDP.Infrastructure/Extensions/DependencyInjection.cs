using Microsoft.Extensions.DependencyInjection;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Infrastructure.Blob;
using FinSolve.IDP.Infrastructure.Cosmos;
using FinSolve.IDP.Infrastructure.Messaging;
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

        var sbNamespace = config["ServiceBusConnection:fullyQualifiedNamespace"] ?? config["ServiceBusConnection__fullyQualifiedNamespace"]
            ?? throw new InvalidOperationException("DEBUG: Missing ServiceBusConnection");

        var storageAccount = config["StorageAccountName"]
            ?? throw new InvalidOperationException("DEBUG: Missing StorageAccountName");

        var cosmosEndpoint = config["CosmosDbAccountEndpoint"]
            ?? throw new InvalidOperationException("DEBUG: Missing CosmosDbAccountEndpoint");


        var cosmosDbName = config["Cosmos:Database"] ?? config["Cosmos__Database"]
            ?? throw new InvalidOperationException("DEBUG: Missing Cosmos__Database in App Settings");

        var statusContainer = config["Cosmos:StatusContainer"] ?? config["Cosmos__StatusContainer"]
            ?? throw new InvalidOperationException("DEBUG: Missing Cosmos__StatusContainer");

        var resultContainer = config["Cosmos:ResultContainer"] ?? config["Cosmos__ResultContainer"]
            ?? throw new InvalidOperationException("DEBUG: Missing Cosmos__ResultContainer");

        var dlqContainer = config["Cosmos:DlqContainer"] ?? config["Cosmos__DlqContainer"]
            ?? throw new InvalidOperationException("DEBUG: Missing Cosmos__DlqContainer");


        services.AddSingleton(sp => new CosmosClient(cosmosEndpoint, new DefaultAzureCredential(),
            new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            }));

        services.AddSingleton<IDocumentHashRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(cosmosDbName, statusContainer);
            return new DocumentHashRepository(container);
        });

        services.AddSingleton<IDocumentStatusRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(cosmosDbName, statusContainer);
            return new DocumentStatusRepository(container);
        });

        services.AddSingleton<IProcessingResultRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(cosmosDbName, resultContainer);
            return new ProcessingResultRepository(container);
        });

        services.AddSingleton<IDlqRepository>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            var container = client.GetContainer(cosmosDbName, dlqContainer);
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
        services.AddSingleton<ILoggingAdapter, ApplicationInsightsLogger>();

        return services;
    }
}