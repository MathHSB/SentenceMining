using Refit;
using SentenceMining.Common;
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
        private readonly IBlobService _blobService;
        private readonly ILogger<AnkiNoteService> _logger;

        public AnkiNoteService(IOpenAIService openAIService, 
            IAnkiConnectApi ankiConnectApi,
            IBlobService blobService,
            ILogger<AnkiNoteService> logger)
        {
            _openAIService = openAIService;
            _ankiConnectApi = ankiConnectApi;
            _blobService = blobService;
            _logger = logger;
        } 

        public async Task AddNote(IFormFile file)
        {
            if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) || file.Length == 0)
            {
                _logger.LogError("Invalid file format or empty file.");
                throw new InvalidOperationException("Invalid file format or empty file.");
            }
            var sentencesMeaning = await _openAIService.GetSentencesMeaning(file);

            var sentencesFormated = GetSentencesFormated(sentencesMeaning);
            var sentencesAudio = await GetSentencesAudio(sentencesFormated);

            await UploadSentenceAudio(sentencesAudio);
            var ankiNotes = CreateAnkiNotes(sentencesAudio);

            await AddNoteInBatches(ankiNotes);

        }

        private async Task<IEnumerable<SentenceAudio>> GetSentencesAudio(
            Dictionary<string, string> sentencesFormated)
        {
            var sentencesAudio = await Task.WhenAll(sentencesFormated.Select(async sentence =>
            {
                var sentenceAudio = await _openAIService.GetSentenceAudio(sentence.Key);
                return new SentenceAudio(sentence.Key, sentence.Value, sentenceAudio.audioBinary, sentenceAudio.name);
            }));

            return sentencesAudio;
        }

        private async Task UploadSentenceAudio(
            IEnumerable<SentenceAudio> sentencesAudio)
        {
            await Task.WhenAll(sentencesAudio.Select(async sentenceAudio =>
            {
                await _blobService.UploadAsync("audio/mpeg", sentenceAudio.AudioBinary, sentenceAudio.Name);
            }));      
        }

        private async Task AddNoteInBatches(
            List<AnkiNoteRequest> ankiNotes)
        {
            try
            {
                var batchSize = 5;
                var numberOfBatches = (int)Math.Ceiling((double)ankiNotes.Count / batchSize);

                for (int i = 0; i < numberOfBatches; i++)
                {
                    var currentNotes = ankiNotes.Skip(i * batchSize).Take(batchSize);
                    var tasks = currentNotes.Select(note => _ankiConnectApi.CreateNote(note));
                    await Task.WhenAll(tasks);
                }

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error: {ex.Message}");
                throw;
            }
        }

        private Dictionary<string, string> GetSentencesFormated(string text)
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

        private List<AnkiNoteRequest> CreateAnkiNotes(
            IEnumerable<SentenceAudio> sentencesAudio)
        {
            var ankiNotes = sentencesAudio.Select(result => new AnkiNoteRequest
            {
                Params = new Params
                {
                    Note = new Note
                    {
                        DeckName = "English",
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
                                Url = BlobHelper.GetBlobAudioPath(result.Name),
                                Filename = result.Name,
                                Fields = new List<string> { "Front" }
                            }
                        }
                    }
                }
            }).ToList();

            return ankiNotes;
        }

    }
}
