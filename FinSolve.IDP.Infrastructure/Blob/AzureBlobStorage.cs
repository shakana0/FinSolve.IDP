using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FinSolve.IDP.Application.Interfaces;

namespace FinSolve.IDP.Infrastructure.Blob
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly BlobContainerClient _container;

        public AzureBlobStorage(BlobContainerClient container)
        {
            _container = container;
        }

        public async Task<string> UploadAsync(
            string blobName,
            byte[] content,
            IDictionary<string, string> metadata)
        {
            var blobClient = _container.GetBlobClient(blobName);

            using var stream = new MemoryStream(content);

            var options = new BlobUploadOptions
            {
                Metadata = metadata,
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "application/pdf"
                }
            };

            await blobClient.UploadAsync(stream, options);

            return blobClient.Uri.ToString();
        }
    }
}
