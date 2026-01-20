using FinSolve.IDP.Domain.Interfaces;

namespace FinSolve.IDP.Application.Services.Idempotency
{
    public class IdempotencyService
    {
        private readonly IDocumentHashRepository _repository;

        public IdempotencyService(IDocumentHashRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsDuplicateAsync(string content)
        {
            var hash = HashGenerator.Compute(content);

            var exists = await _repository.ExistsAsync(hash);

            if (exists)
                return true;

            await _repository.SaveAsync(hash);
            return false;
        }
    }
}
