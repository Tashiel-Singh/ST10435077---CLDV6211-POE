using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ST10435077___CLDV6211_POE.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _containerClient = _blobServiceClient.GetBlobContainerClient("venue-images");
            _containerClient.CreateIfNotExists(PublicAccessType.BlobContainer);
        }

        public async Task<string?> UploadAsync(IFormFile file)
        {
            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var blobClient = _containerClient.GetBlobClient(uniqueFileName);
                await blobClient.UploadAsync(file.OpenReadStream(), true);
                return uniqueFileName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<string>> ListBlobsAsync()
        {
            return await ExecuteWithRetryAsync(async () =>
            {
                var blobs = new List<string>();
                await foreach (var blobItem in _containerClient.GetBlobsAsync())
                {
                    blobs.Add(blobItem.Name);
                }
                return blobs;
            });
        }

        public async Task<(Stream? Stream, string? ContentType)> DownloadAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                if (!await blobClient.ExistsAsync())
                    return (null, null);
                var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                var properties = await blobClient.GetPropertiesAsync();
                return (memoryStream, properties.Value.ContentType);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                return await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception) when (i < 2)
                {
                    await Task.Delay(1000 * (i + 1));
                }
            }
            throw new Exception("Operation failed after 3 retry attempts");
        }
    }
}
