using OpenAI.Audio;
using OpenAI.Chat;
using SentenceMining.Services.Abstraction;

namespace SentenceMining.Services
{
    public sealed class OpenAIService : IOpenAIService
    {
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(ILogger<OpenAIService> logger) => _logger = logger;
        
        private const string Prompt = "I want you to create sentences using the N+1 technique. This means:" +
            "\r\n- Each sentence should contain one unknown word or phrase (bold the word or phrase using `<b>` tags without asterisks)." +
            "\r\n- I should know the meaning of all the other words in the sentence except the bolded '<b>' tag one." +
            "\r\n\r\n" +"Provide the meaning of the unknown word or phrase immediately below the sentence, using the exact Front/Back format as follows:" +
            "\r\n\r\nExample format for output:\r\nFront: Desperately, she went on to defend her son from all dangers." +
            "\r\nBack: (Continuou/prosseguiu.)\r\n\r\nYour task:\r\n1. I will provide a list of words or phrases." +
            "\r\n2. For each word or phrase in the list:\r\n   - Create a simple/intermediate sentence using the N+1 technique." +
            "\r\n   - Use the unknown word or phrase in the context of the sentence.\r\n   - Write the sentence in the format:" +
            "\r\n     Front: [Sentence containing the unknown word or phrase, with the unknown part in bold using `<b>` tags without asterisks.]" +
            "\r\n     Back: [Meaning or meanings of the unknown word or phrase in parentheses.]\r\n\r\nImportant instructions:" +
            "\r\n- Do not alter or split the input words or phrases; process them exactly as they are provided." +
            "\r\n- Do not modify the meaning of the words or phrases.\r\n- Ensure the sentence is written in a simple and clear way." 
            + "- If the input contains multiple words (e.g., \"makes actually\"), treat them as a single, inseparable unit.\n"+
            "\r\n- Separate each Front/Back pair with a blank line for readability.\r\n\r\nHere’s the list of words or phrases:\r\n";

        public async Task<string> GetSentencesMeaning(IFormFile file)
        {
            try
            {
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
