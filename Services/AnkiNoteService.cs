using Refit;
using SentenceMining.Dtos;
using SentenceMining.Infra.Abstraction;
using SentenceMining.Services.Abstraction;
using System.Text.RegularExpressions;
namespace SentenceMining.Services
{
    public sealed class AnkiNoteService : IAnkiNoteService
    {
        private readonly IOpenAIService _openAIService;
        private readonly IAnkiConnectApi _ankiConnectApi;

        public AnkiNoteService(IOpenAIService openAIService, 
            IAnkiConnectApi ankiConnectApi )
        {
           _openAIService = openAIService;
            _ankiConnectApi = ankiConnectApi;
        } 

        public async Task AddNote(IFormFile file)
        {
            using StreamReader reader = new(file.OpenReadStream());

            var sentencesMeaning = await _openAIService.GetSentencesMening(reader);
            var sentencesDictionary = GetSentenceDictionary(sentencesMeaning);

            var sentencesAudio = sentencesDictionary.Select(async sentence =>
            {
                var audio = await _openAIService.GetSentenceAudio(sentence.Key);
                return (sentence.Key, sentence.Value, audio.stream, audio.name);
            });

            var audioResults = await Task.WhenAll(sentencesAudio);

            var audioTestes = audioResults.Take(5).ToList();

            var ankiNotes = audioTestes.Select(result => new AnkiNote
            {
                Params = new Params
                {
                    Note = new Note
                    {
                        DeckName = "Default",
                        ModelName = "Basic",
                        Fields = new Fields
                        {
                            Front = result.Key,
                            Back = result.Value
                        },
                        Audio = new List<Audio>
                {
                    new Audio
                    {
                        Url = result.stream.Name,
                        Filename = result.name,
                        Fields = new List<string> { "Front" }
                    }
                }
                    }
                }
            }).ToList();

            var responses = await AddNoteInBatches(ankiNotes);
        }

        private async Task<List<ApiResponse<AnkiNoteResponse>>> AddNoteInBatches(List<AnkiNote> ankiNotes)
        {
            var responses = new List<ApiResponse<AnkiNoteResponse>>();
            var batchSize = 10;

            var numberOfBatches = (int)Math.Ceiling((double)ankiNotes.Count / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentNotes = ankiNotes.Skip(i * batchSize).Take(batchSize);
                var tasks = currentNotes.Select(note => _ankiConnectApi.CreateNote(note));
                responses.AddRange(await Task.WhenAll(tasks));
            }

            return responses;
        }

        private Dictionary<string, string> GetSentenceDictionary(string text)
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
