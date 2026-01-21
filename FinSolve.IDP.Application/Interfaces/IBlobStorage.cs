namespace FinSolve.IDP.Application.Interfaces
{
    public interface IBlobStorage
    {
        Task<string> UploadAsync(
            string blobName,
            byte[] content,
            IDictionary<string, string> metadata);
    }
}
