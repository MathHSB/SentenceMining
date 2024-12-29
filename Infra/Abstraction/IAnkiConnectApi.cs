using Refit;
using SentenceMining.Dtos;

namespace SentenceMining.Infra.Abstraction
{
    public interface IAnkiConnectApi
    {
        [Post("/")]
        Task<ApiResponse<AnkiNoteResponse>> CreateNote([Body] AnkiNote ankiNote);
    }
}
