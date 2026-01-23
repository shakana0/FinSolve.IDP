using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Infrastructure.Blob;
using FinSolve.IDP.Infrastructure.Cosmos;
using FinSolve.IDP.Infrastructure.Dependencies.Cosmos;
using FinSolve.IDP.Infrastructure.KeyVault;
using FinSolve.IDP.Infrastructure.Logging;
using FinSolve.IDP.Infrastructure.Messaging;
using FinSolve.IDP.Infrastructure.Pdf;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Cosmos;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        // Cosmos DB
        services.AddSingleton<CosmosClient>(sp =>
            new CosmosClient(config["Cosmos:ConnectionString"]));

        services.AddSingleton<Container>(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            return client.GetContainer(config["Cosmos:Database"], config["Cosmos:Container"]);
        });

        services.AddSingleton<IProcessingResultRepository, ProcessingResultRepository>();
        services.AddSingleton<IDocumentHashRepository, DocumentHashRepository>();

        // Blob Storage
        services.AddSingleton<BlobServiceClient>(sp =>
            new BlobServiceClient(config["BlobStorage:ConnectionString"]));

        services.AddSingleton<BlobContainerClient>(sp =>
        {
            var service = sp.GetRequiredService<BlobServiceClient>();
            return service.GetBlobContainerClient(config["BlobStorage:ContainerName"]);
        });

        services.AddSingleton<IBlobStorage, AzureBlobStorage>();

        // Service Bus
        services.AddSingleton<ServiceBusClient>(sp =>
            new ServiceBusClient(config["ServiceBus:ConnectionString"]));

        services.AddSingleton<IMessagePublisher, ServiceBusMessagePublisher>();

        // PDF
        services.AddSingleton<IPdfGenerator, QuestPdfGenerator>();

        // Key Vault
        services.AddSingleton<IKeyVaultSecretProvider>(sp =>
        {
            var keyVaultUrl = config["KeyVault:Url"];
            if (string.IsNullOrEmpty(keyVaultUrl))
            {
                throw new InvalidOperationException("KeyVault:Url configuration is missing");
            }
            return new KeyVaultSecretProvider(keyVaultUrl);
        });

        services.AddSingleton<IDocumentStatusRepository, DocumentStatusRepository>();
        services.AddSingleton<IDlqRepository, DlqRepository>();

        // Application Insights
        services.AddSingleton<TelemetryConfiguration>(sp =>
            TelemetryConfiguration.CreateDefault());

        services.AddSingleton<TelemetryClient>();
        services.AddSingleton<ILoggingAdapter, ApplicationInsightsLogger>();
    })
    .Build();

host.Run();
