namespace SentenceMining.Infra.Abstraction
{
    public interface IBlobService
    {
        Task UploadAsync(string contentType, BinaryData audioBinary, string audioName, CancellationToken cancellationToken = default);
    }
}
