using Carter;
using SentenceMining.Services.Abstraction;

namespace SentenceMining.Endpoints
{
    public class AnkiNoteEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/createNote", async (
                IAnkiNoteService ankiNoteService, 
                IFormFile file) =>
            {
                await ankiNoteService.AddNote(file);
                return Results.Created("/addNote", new { Message = "Notes created successfully"});
            })
            .WithName("AddNoteEndpoint")
            .DisableAntiforgery();
        }
    }
}
