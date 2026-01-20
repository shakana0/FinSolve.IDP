using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Domain.Entities;
using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Application.Mappers
{
    public static class CosmosDocumentMapper
    {
        public static ProcessingResultCosmosDto ToCosmosDocument(ProcessingResult result)
        {
            return new ProcessingResultCosmosDto
            {
                id = result.DocumentId.Value.ToString(),
                DocumentId = result.DocumentId.Value.ToString(),
                PrimaryCategory = result.PrimaryCategory,
                Items = result.Items.ToList(),
                Summary = result.Summary,
                PartitionKey = result.DocumentId.Value.ToString(),
                CreatedUtc = DateTime.UtcNow
            };
        }

        public static ProcessingResult ToDomain(ProcessingResultCosmosDto dto)
        {
            return new ProcessingResult(
                new DocumentId(Guid.Parse(dto.DocumentId)),
                dto.PrimaryCategory,
                dto.Items,
                dto.Summary
            );
        }
    }
}
