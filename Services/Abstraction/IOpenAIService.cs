namespace SentenceMining.Services.Abstraction
{
    public interface IOpenAIService
    {
        Task<string> GetSentencesMening(StreamReader reader);
        Task<bool> GetSentenceAudio(string front);
    }
}
