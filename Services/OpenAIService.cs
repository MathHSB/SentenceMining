using OpenAI.Audio;
using OpenAI.Chat;
using SentenceMining.Services.Abstraction;

namespace SentenceMining.Services
{
    public sealed class OpenAIService : IOpenAIService
    {
        private readonly ILogger<OpenAIService> _logger;
        public OpenAIService(ILogger<OpenAIService> logger)
        {
            _logger = logger;
        }
        
        private const string Prompt = "I want sentences where I know the meaning of all the words except for one (N+1). "
                + "The unknown word or phrase should be used in the context of the simple/intermediate sentence. "
                + "Below the sentence, provide the meaning of the unknown word or phrase in the exact Front/Back format like this:"
                + "\r\n\r\nExample format for output:\r\n" 
                +"Front: Desperately, she went on to defend her son from all dangers."
                + "\r\nBack: (Continuou/prosseguiu.)\r\n\r\nWhat you should do:"
                + "\r\nI will provide you with a list of words or phrases. For each word, create simple/intermediate sentences using the N+1 technique."
                + "Write the output in the following format:\r\n\r\n"
                + "Front: [Sentence containing the unknown word(in bold using tag <b> ) or phrase]\r\n"
                + "Back: [Meaning or meanings if exists of the unknown word or phrase in parentheses]"
                + "\r\nHere’s the list of words:\r\n";

        public async Task<string> GetSentencesMeaning(IFormFile file)
        {
            try
            {
                throw new HttpRequestException("Erro teste");
                using StreamReader fileReader = new(file.OpenReadStream());
                ChatClient client = new(model: "gpt-4o-mini", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                ChatCompletion completion = client.CompleteChat($"{Prompt}{await fileReader.ReadToEndAsync()}");

                return completion.Content[0].Text;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error: {ex.Message}");
                throw;
            }       
        }

        public async Task<(BinaryData, string)> GetSentenceAudio(string front)
        {
            try
            {
                Directory.CreateDirectory("Audio");
                var audioName = $"{Guid.NewGuid()}.mp3";
                var filePath = Path.Combine("Audio", audioName);

                AudioClient client = new("tts-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                BinaryData speech = await client.GenerateSpeechAsync(front, GeneratedSpeechVoice.Echo);

                using FileStream stream = File.OpenWrite(filePath);
                speech.ToStream().CopyTo(stream);

                return (speech, audioName);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP error: {ex.Message}");
                throw;
            }          
        }
    }
}
