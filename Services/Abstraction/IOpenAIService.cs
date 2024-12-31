namespace SentenceMining.Services.Abstraction
{
    public interface IOpenAIService
    {
        Task<string> GetSentencesMening(IFormFile reader);

        Task<(BinaryData audioBinary, string name)> GetSentenceAudio(string front);
    }
}
