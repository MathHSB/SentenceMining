using SentenceMining.Services.Abstraction;
namespace SentenceMining.Services
{
    public sealed class AnkiNoteService : IAnkiNoteService
    {
        private readonly IOpenAIService _openAIService;
        public AnkiNoteService(IOpenAIService openAIService) => _openAIService = openAIService;     

        public async Task AddNote(IFormFile file)
        {
            using StreamReader reader = new(file.OpenReadStream());
            var text = await _openAIService.GetSentencesMening(reader);
            Console.WriteLine($"[ASSISTANT]: {text}");
          
        }
    }
}
