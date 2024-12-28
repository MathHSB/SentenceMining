using Refit;
using SentenceMining.Dtos;

namespace SentenceMining.Infra.Abstraction
{
    public interface IAnkiConnectApi
    {
        [Post("/")]
        Task<HttpResponseMessage> CreateNote([Body] AnkiNote ankiNote);
    }
}
