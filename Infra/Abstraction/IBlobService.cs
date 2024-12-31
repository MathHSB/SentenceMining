namespace SentenceMining.Infra.Abstraction
{
    public interface IBlobService
    {
        Task UploadAsync(string contentType, BinaryData audioBinary, CancellationToken cancellationToken = default);
    }
}
