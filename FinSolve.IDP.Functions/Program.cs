using FinSolve.IDP.Application.Extensions;
using FinSolve.IDP.Application.Interfaces;
using FinSolve.IDP.Infrastructure.Extensions;
using FinSolve.IDP.Infrastructure.KeyVault;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;


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

        services.AddApplicationServices();
        services.AddInfrastructureServices(config);

        // Key Vault
        services.AddSingleton<IKeyVaultSecretProvider>(sp =>
        {
            var keyVaultUrl = config["KeyVaultUri"];

            if (string.IsNullOrEmpty(keyVaultUrl))
            {
                throw new InvalidOperationException("Konfigurationsvärdet 'KeyVaultUri' saknas i appsettings eller miljövariabler.");
            }

            return new KeyVaultSecretProvider(keyVaultUrl);
        });

        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService(options =>
        {
            options.EnablePerformanceCounterCollectionModule = false;
            options.EnableDependencyTrackingTelemetryModule = false;
        });

        services.ConfigureFunctionsApplicationInsights();

        // Sampling
        // services.Configure<TelemetryConfiguration>(config =>
        // {
        //     var builder = config.DefaultTelemetrySink.TelemetryProcessorChainBuilder;
        //     builder.UseSampling(5.0);
        //     builder.Build();
        // });

    })
    .Build();

host.Run();
