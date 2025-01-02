using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SentenceMining.Infra.Abstraction;
namespace SentenceMining.Infra.Storage
{
    public sealed class BlobService : IBlobService
    {
        private const string ContainerName = "sentences-audio";
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobService> _logger;

        public BlobService(BlobServiceClient blobServiceClient, 
            ILogger<BlobService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task UploadAsync( 
            string contentType,
            BinaryData audioBinary,
            string audioName,
            CancellationToken cancellationToken = default)
        {

            try
            {
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                BlobClient blobClient = containerClient.GetBlobClient(audioName);

                await blobClient.UploadAsync(
                    audioBinary.ToStream(),
                    new BlobHttpHeaders { ContentType = contentType },
                    cancellationToken: cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error: {ex.Message}");
                throw;
            }
          
        }
    }
}
