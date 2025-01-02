using Azure.Storage.Blobs;
using Carter;
using Refit;
using SentenceMining.Infra.Abstraction;
using SentenceMining.Infra.Http;
using SentenceMining.Infra.Storage;
using SentenceMining.Services;
using SentenceMining.Services.Abstraction;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


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
builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddSingleton(new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapCarter();

app.MapControllers();

app.Run();
