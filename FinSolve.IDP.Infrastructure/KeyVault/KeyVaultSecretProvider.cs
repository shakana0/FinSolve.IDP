using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FinSolve.IDP.Application.Interfaces;

namespace FinSolve.IDP.Infrastructure.KeyVault
{
    public class KeyVaultSecretProvider : IKeyVaultSecretProvider
    {
        private readonly SecretClient _secretClient;

        public KeyVaultSecretProvider(string keyVaultUrl)
        {
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }

        public async Task<string?> GetSecretAsync(string secretName)
        {
            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
                return secret.Value;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }
    }
}
