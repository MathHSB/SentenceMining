namespace SentenceMining.Dtos
{
    public record SentenceAudio(
        string Key, 
        string Value, 
        BinaryData AudioBinary, 
        string Name);
}
