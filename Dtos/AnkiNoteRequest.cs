using Newtonsoft.Json;

namespace SentenceMining.Dtos
{
    public class AnkiNoteRequest
    {
        [JsonProperty("action")]
        public string Action { get;} = "addNote";

        [JsonProperty("version")]
        public int Version { get;} = 6;

        [JsonProperty("params")]
        public Params Params { get; set; } = default!;
    }
    public class Params
    {
        [JsonProperty("note")]
        public Note Note { get; set; } = default!;
    }
    public class Note
    {
        [JsonProperty("deckName")]
        public string DeckName { get; set; } = default!;

        [JsonProperty("modelName")]
        public string ModelName { get; set; } = default!;

        [JsonProperty("fields")]
        public Fields Fields { get; set; } = default!;

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = default!;

        [JsonProperty("audio")]
        public List<Audio> Audio { get; set; } = default!;
    }

    public class Fields
    {
        [JsonProperty("Front")]
        public string Front { get; set; } = default!;

        [JsonProperty("Back")]
        public string Back { get; set; } = default!;
    }

    public class Audio
    {
        [JsonProperty("url")]
        public string Url { get; set; } = default!;

        [JsonProperty("filename")]
        public string Filename { get; set; } = default!;

        [JsonProperty("skipHash")]
        public string SkipHash { get; set; } = default!;

        [JsonProperty("fields")]
        public List<string> Fields { get; set; } = default!;
    }
}
