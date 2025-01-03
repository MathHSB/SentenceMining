namespace SentenceMining.Services.Abstraction
{
    public interface IAnkiNoteService
    {
        Task AddNote(IFormFile file);
    }
}
