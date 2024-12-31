using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SentenceMining.Infra.Abstraction;

namespace SentenceMining.Infra.Storage
{
    public sealed class BlobService(BlobServiceClient blobServiceClient) : IBlobService
    {
        private const string ContainerName = "sentences-audio";

        public async Task UploadAsync( 
            string contentType,
            BinaryData audioBinary,
            CancellationToken cancellationToken = default)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            BlobClient blobClient = containerClient.GetBlobClient($"{Guid.NewGuid()}.mp3");

            await blobClient.UploadAsync(
                audioBinary.ToStream(),
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken);

        }
    }
}
