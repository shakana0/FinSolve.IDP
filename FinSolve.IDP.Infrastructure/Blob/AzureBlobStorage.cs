using Azure.Identity;
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

        public async Task<byte[]> DownloadAsync(string blobPath)
        {
            var client = _container.GetBlobClient(blobPath);

            using var ms = new MemoryStream();
            await client.DownloadToAsync(ms);
            return ms.ToArray();
        }

        public async Task<Stream> DownloadStreamAsync(string blobPath)
        {
            BlobClient blobClient;

            if (Uri.TryCreate(blobPath, UriKind.Absolute, out var uri))
            {
                blobClient = new BlobClient(uri, new DefaultAzureCredential());
            }
            else
            {
                blobClient = _container.GetBlobClient(blobPath);
            }

            return await blobClient.OpenReadAsync();
        }

    }
}
