using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace BusinessLogicLayer.Services.BlobService
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient? _blobServiceClient;
        private readonly string _localUploadPath;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = configuration["AZURE_STORAGE_CONNECTION_STRING"];
            if (!string.IsNullOrEmpty(connectionString))
                _blobServiceClient = new BlobServiceClient(connectionString);

            _localUploadPath = Path.Combine(AppContext.BaseDirectory, "uploads");
            Directory.CreateDirectory(_localUploadPath);
        }

        public async Task<string> UploadFileAsync(Stream stream, string fileName, string containerName)
        {
            if (_blobServiceClient != null)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                var uniqueName = $"{Guid.NewGuid()}_{fileName}";
                var blobClient = containerClient.GetBlobClient(uniqueName);
                await blobClient.UploadAsync(stream, overwrite: true);
                return blobClient.Uri.ToString();
            }

            var dir = Path.Combine(_localUploadPath, containerName);
            Directory.CreateDirectory(dir);
            var localName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var localPath = Path.Combine(dir, localName);
            using var fs = File.Create(localPath);
            await stream.CopyToAsync(fs);
            return $"/uploads/{containerName}/{localName}";
        }

        public async Task DeleteFileAsync(string blobUrl)
        {
            if (_blobServiceClient != null && blobUrl.StartsWith("http"))
            {
                var uri = new Uri(blobUrl);
                var segments = uri.Segments;
                if (segments.Length < 2) return;
                var containerName = segments[1].TrimEnd('/');
                var blobName = string.Concat(segments[2..]);
                await _blobServiceClient
                    .GetBlobContainerClient(containerName)
                    .GetBlobClient(blobName)
                    .DeleteIfExistsAsync();
                return;
            }

            if (blobUrl.StartsWith("/uploads/"))
            {
                var localPath = Path.Combine(_localUploadPath, blobUrl.Replace("/uploads/", "").Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(localPath)) File.Delete(localPath);
            }
        }
    }
}
