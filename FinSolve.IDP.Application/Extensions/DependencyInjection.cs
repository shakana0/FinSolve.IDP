using FinSolve.IDP.Application.Services.DocumentMetadata;
using FinSolve.IDP.Application.Services.DocumentProcessing;
using FinSolve.IDP.Application.Services.Idempotency;
using FinSolve.IDP.Application.Services.Pdf;
using FinSolve.IDP.Application.Services.Summery;
using FinSolve.IDP.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace FinSolve.IDP.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Metadata Extraction
        services.AddScoped<MetadataExtractionService>();
        services.AddScoped<IDocumentValidator, DocumentValidator>();

        // Document Processing
        services.AddScoped<DocumentProcessingService>();
        services.AddScoped<DocumentFormatDetector>();
        services.AddScoped<DocumentReaderFactory>();

        // Pipeline Services
        services.AddScoped<IdempotencyService, IdempotencyService>();
        services.AddScoped<PdfGenerationService>();
        services.AddScoped<SummaryGeneratorService>();

        return services;
    }
}