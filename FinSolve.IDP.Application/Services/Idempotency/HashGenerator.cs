using System.Security.Cryptography;
using System.Text;
using FinSolve.IDP.Domain.ValueObjects;

namespace FinSolve.IDP.Application.Services.Idempotency
{
    public static class HashGenerator
    {
        public static Hash Compute(string content)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(content);
            var hashBytes = sha.ComputeHash(bytes);
            return Hash.FromBytes(hashBytes);
        }
    }
}
