using FinSolve.IDP.Application.DTOs;
using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Application.Mappers
{
    public static class ProcessingResultMapper
    {
        public static ProcessingMessageDto ToMessage(ProcessingResult result)
        {
            return new ProcessingMessageDto
            {
                DocumentId = result.DocumentId.Value,
                PrimaryCategory = result.PrimaryCategory,
                Items = result.Items.ToList(),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
