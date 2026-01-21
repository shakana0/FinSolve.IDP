namespace FinSolve.IDP.Application.Interfaces
{
    public interface IKeyVaultSecretProvider
    {
        Task<string?> GetSecretAsync(string secretName);
    }
}
