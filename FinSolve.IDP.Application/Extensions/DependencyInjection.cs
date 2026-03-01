using FinSolve.IDP.Application.Services.DocumentMetadata;
using FinSolve.IDP.Application.Services.DocumentProcessing;
using FinSolve.IDP.Application.Services.Idempotency;
using FinSolve.IDP.Application.Services.Pdf;
using FinSolve.IDP.Application.Services.Summery;
using FinSolve.IDP.Domain.Interfaces;
using FinSolve.IDP.Domain.Readers;
using Microsoft.Extensions.DependencyInjection;


namespace FinSolve.IDP.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 1. Register Detector first
        services.AddScoped<DocumentFormatDetector>();

        // 2. Register Factory (required by MetadataExtractionService)
        services.AddScoped<DocumentReaderFactory>();

        // 3. Register Validator
        services.AddScoped<IDocumentValidator, DocumentValidator>();

        // 4. Register Service (requested by your Azure Function)
        services.AddScoped<MetadataExtractionService>();

        // Pipeline Services
        services.AddScoped<IdempotencyService>();
        services.AddScoped<PdfGenerationService>();
        services.AddScoped<SummaryGeneratorService>();

        // Register all readers as IDocumentReader for IEnumerable injection
        services.AddSingleton<IDocumentReader, TxtDocumentReader>();
        services.AddSingleton<IDocumentReader, CsvDocumentReader>();
        services.AddSingleton<IDocumentReader, PdfDocumentReader>();
        services.AddSingleton<IDocumentReader, DocxDocumentReader>();

        return services;
    }
}