using System.Text.Json.Serialization;

namespace SentenceMining.Dtos
{
    public class AnkiNoteResponse
    {
        [JsonPropertyName("result")]
        public long Result { get; set; } 

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
