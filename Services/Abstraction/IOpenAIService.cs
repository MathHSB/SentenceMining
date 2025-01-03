namespace SentenceMining.Services.Abstraction
{
    public interface IOpenAIService
    {
        Task<string> GetSentencesMeaning(IFormFile reader);

        Task<(BinaryData audioBinary, string name)> GetSentenceAudio(string front);
    }
}
