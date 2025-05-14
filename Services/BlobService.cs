using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
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

        public async Task<string> UploadAsync(IFormFile file)
        {
            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}-{file.FileName}";
                var blobClient = _containerClient.GetBlobClient(uniqueFileName);
                await blobClient.UploadAsync(file.OpenReadStream(), true);
                return blobClient.Uri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<string>> ListBlobsAsync()
        {
            try
            {
                var blobs = new List<string>();
                await foreach (var blobItem in _containerClient.GetBlobsAsync())
                {
                    blobs.Add(blobItem.Name);
                }
                return blobs;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<(Stream Stream, string ContentType)> DownloadAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                var properties = await blobClient.GetPropertiesAsync();
                return (memoryStream, properties.Value.ContentType);
            }
            catch (Exception)
            {
                return (new MemoryStream(), "application/octet-stream");
            }
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
