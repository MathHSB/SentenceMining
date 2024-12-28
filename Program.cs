using Carter;
using Refit;
using SentenceMining.Infra.Abstraction;
using SentenceMining.Infra.Http;
using SentenceMining.Services;
using SentenceMining.Services.Abstraction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();


builder.Services
    .AddRefitClient<IAnkiConnectApi>(new RefitSettings(new NewtonsoftJsonContentSerializer()))
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:8765"))
     .ConfigurePrimaryHttpMessageHandler(() =>
     {
         return new ForceBufferingHandler(new HttpClientHandler
         {
             AllowAutoRedirect = false
         });
     });

builder.Services.AddScoped<IAnkiNoteService, AnkiNoteService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapCarter();

app.MapControllers();

app.Run();
