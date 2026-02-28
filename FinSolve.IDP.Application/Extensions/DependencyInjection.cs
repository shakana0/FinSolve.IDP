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
        // Metadata & Validation
        services.AddScoped<MetadataExtractionService>();
        services.AddScoped<IDocumentValidator, DocumentValidator>();

        // Document Processing
        services.AddScoped<DocumentProcessingService>();
        services.AddScoped<DocumentFormatDetector>();

        // Register factory as Singleton to manage the readers list
        services.AddSingleton<DocumentReaderFactory>();

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