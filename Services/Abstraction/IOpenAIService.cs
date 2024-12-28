namespace SentenceMining.Services.Abstraction
{
    public interface IOpenAIService
    {
        Task<string> GetSentencesMening(StreamReader reader);
        Task<FileStream> GetSentenceAudio(string front);
    }
}
