namespace SentenceMining.Services.Abstraction
{
    public interface IOpenAIService
    {
        Task<string> GetSentencesMening(StreamReader reader);
        Task<(FileStream stream, string name)> GetSentenceAudio(string front);
    }
}
