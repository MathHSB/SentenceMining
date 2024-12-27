using SentenceMining.Services.Abstraction;
using System.Text.RegularExpressions;
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
            var sentences = GetSentenceKeys(text);

            var audioTasks = sentences
                .Select(sentence => _openAIService.GetAudioSentence(sentence.Key));

            await Task.WhenAll(audioTasks);
        }

        private Dictionary<string, string> GetSentenceKeys(string text)
        {
            var dictionary = new Dictionary<string, string>();

            var regex = new Regex(@"Front: (.+?)\s+Back: \((.+?)\)",
            RegexOptions.Singleline);

            var matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                string front = match.Groups[1].Value.Trim();
                string back = match.Groups[2].Value.Trim();
                dictionary[front] = back;
            }
             return dictionary;
        }
    }
}
