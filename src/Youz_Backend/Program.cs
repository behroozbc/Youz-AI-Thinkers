using System.Text;
using Mapster;

using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Text;
using NetTopologySuite.Geometries;
using Npgsql;
using Youz_Backend.DB;
using Youz_Backend.DB.Models;
using Youz_Backend.Dtos;

var builder = WebApplication.CreateBuilder(args);
#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMapster();
var config = new TypeAdapterConfig();
config.Scan(typeof(MappingConfig).Assembly);

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddScoped<ImageEmbeddingService>();
var mic = builder.Configuration.GetSection(nameof(AiSystemConfiguration)).Get<AiSystemConfiguration>();
builder.Services.AddGoogleAIGeminiChatCompletion(modelId: mic.ModelId, apiKey: mic.ApiKey);
builder.Services.AddGoogleAIEmbeddingGenerator(
    modelId: mic.EmbeddingModel,       // Name of the embedding model, e.g. "models/text-embedding-004".
    apiKey: mic.ApiKey
);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
    connectionString,
    o =>
    {
        o.UseNetTopologySuite();
        o.UseVector();
    }));
builder.Services.AddTransient((serviceProvider) =>
{
    return new Kernel(serviceProvider);
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
app.MapPost("/near-location", async (ApplicationDbContext dbContext, PointDto point, int radius = 10) =>
{
    var g = new Point(new Coordinate(point.Latitude, point.Longitude));
    return await dbContext.Landmarks.Where(c => c.Location.Distance(g) > radius).ToListAsync();
}).WithName("GetNearLocation");
app.MapPost("/landmark/create", async (ApplicationDbContext dbContext, IMapper mapper, Kernel kernel, LandmarkDto landmark) =>
{
    var entity = mapper.Map<Landmark>(landmark);
    entity.DescriptionChunks = [];
    var embedding = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
    var chunks = TextChunker.SplitPlainTextLines(entity.Description, 500);
    foreach (var chunk in await embedding.GenerateAndZipAsync(chunks, new() { Dimensions = 3072 }))
    {
        entity.DescriptionChunks.Add(new()
        {
            Text = chunk.Value,
            TextEmbedding = new Pgvector.Vector(chunk.Embedding.Vector)
        });
    }
    await dbContext.Landmarks.AddAsync(entity);
    var IsOkay = await dbContext.SaveChangesAsync();
    return IsOkay;
})
.WithName("Create Landmark");
app.MapPost("/landmark/{id}/images/create", async (ApplicationDbContext dbContext, IMapper mapper, Kernel kernel, ImageEmbeddingService imageEmbeddingService, Guid id, LandmarkImageDto landmarkImageDto) =>
{
    var landmark = await dbContext.Landmarks.Include(c => c.ImageChunks).FirstAsync(c => c.ID == id);
    landmark.ImageChunks.Add(new()
    {
        Caption = landmarkImageDto.Caption,
        Image = landmarkImageDto.Image,
        ImageEmbedding = await imageEmbeddingService.GetImageEmbedding(landmarkImageDto.Image)
    });
    await dbContext.SaveChangesAsync();
}).WithName("create landmark image");
app.MapPost("/answer-question", async (ApplicationDbContext dbContext, IMapper mapper, Kernel kernel, IChatCompletionService chatCompletionService, UserPromptDto userPrompt) =>
{
    var embedding = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
    var userInputEmb = new Pgvector.Vector(await embedding.GenerateVectorAsync(userPrompt.UserPrompt));
    var g = mapper.Map<Point>(userPrompt.Location);
    var nearLocations = await dbContext.Landmarks.AsNoTracking()
        .Where(c => c.Location.Distance(g) < 100)
        .ToListAsync();
    var contextBuilder = new StringBuilder();
    var sql = $@"
        SELECT * FROM ""DescriptionChunks""
        WHERE ""LandmarkID"" = @landmarkId
          AND ""TextEmbedding"" <=> @embedding < @maxDistance
        ORDER BY ""TextEmbedding"" <=> @embedding
        LIMIT @limit";
    foreach (var nearLocation in nearLocations)
    {
        var results = await dbContext.DescriptionChunks
                .FromSqlRaw(sql,
                    new NpgsqlParameter("@landmarkId", nearLocation.ID),
                    new NpgsqlParameter("@embedding", userInputEmb),
                    new NpgsqlParameter("@maxDistance", 0.25),
                    new NpgsqlParameter("@limit", 3))
                .ToListAsync();
        contextBuilder.AppendJoin(Environment.NewLine, results.Select(c => c.Text));

    }
    var prompt = $@"
    The context of your answer: 
    {contextBuilder.ToString()}
    User prompt:
    {userPrompt.UserPrompt}
    ";
    var chathistory = new ChatHistory();
    chathistory.AddUserMessage(prompt);
    var result = await chatCompletionService.GetChatMessageContentAsync(chathistory);

    return result.Content;
}).WithName("Answer");
app.MapGet("test", (IMapper mapper) =>
{
    return mapper.Map<Point, PointDto>(new Point(new Coordinate(4, 23.5)));
});
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.