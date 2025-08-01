using app.Data;
using app.Services;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.Search.Documents.Indexes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    Console.WriteLine("SQL Database connection string not found. Using in-memory database for development.");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("SampleStoreDb"));
}

// Configure and register Semantic Kernel
var kernelBuilder = builder.Services.AddKernel();

// Configure Azure OpenAI
var openAIConfig = builder.Configuration.GetSection("OpenAI");
var endpoint = openAIConfig["Endpoint"];
var apiKey = openAIConfig["ApiKey"];
var deploymentName = openAIConfig["DeploymentName"] ?? "gpt-4";

if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(apiKey))
{
    // Use Azure OpenAI
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentName,
        endpoint: endpoint,
        apiKey: apiKey);
}
else
{
    Console.WriteLine("Azure OpenAI configuration is missing. Using mock service for development.");
    // Fallback to a mock service for development
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: "gpt-4",
        endpoint: "https://mock-endpoint.openai.azure.com/",
        apiKey: "mock-key");
}

// Configure Azure Vision API
var visionConfig = builder.Configuration.GetSection("AzureVision");
var visionEndpoint = visionConfig["Endpoint"];
var visionApiKey = visionConfig["ApiKey"];

if (!string.IsNullOrEmpty(visionEndpoint) && !string.IsNullOrEmpty(visionApiKey))
{
    // Register Azure Vision client factory
    builder.Services.AddSingleton(sp =>
    {
        return new ImageAnalysisClientFactory(visionEndpoint, visionApiKey);
    });
}
else
{
    Console.WriteLine("Azure Vision API configuration is missing. Using mock service for development.");
    // Log a warning if vision API is not configured
    builder.Services.AddSingleton<ImageAnalysisClientFactory>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<ImageAnalysisClientFactory>>();
        logger.LogWarning("Azure Vision API not configured. Image analysis will not work properly.");
        return new ImageAnalysisClientFactory("https://mock-endpoint.cognitiveservices.azure.com/", "mock-key");
    });
}

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddAzureOpenAIEmbeddingGenerator(
    "text-embedding-3-small",
    endpoint,
    apiKey
    );
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

// Azure AI Search
var aiSearchConfig = builder.Configuration.GetSection("AzureAISearch");
var aiSearchEndpoint = aiSearchConfig["Endpoint"];
var aiSearchApiKey = aiSearchConfig["ApiKey"];
var aiSearchIndexName = aiSearchConfig["IndexName"] ?? "products";

var searchIndexClient = new SearchIndexClient(
    new Uri(aiSearchEndpoint),
    new AzureKeyCredential(aiSearchApiKey)
);
builder.Services.AddSingleton(searchIndexClient);

builder.Services.AddSingleton(sp =>
{
    return new AzureAISearchCollection<string, ProductVector>(
        searchIndexClient,
        aiSearchIndexName);
});

// Register services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<VectorService>();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    try
    {
        // Use migrations if using SQL Server, otherwise EnsureCreated for in-memory
        if (dbContext.Database.IsSqlServer())
        {
            dbContext.Database.Migrate();
        }
        else
        {
            dbContext.Database.EnsureCreated();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        // For development, if migration fails, try EnsureCreated as fallback
        if (app.Environment.IsDevelopment())
        {
            logger.LogWarning("Attempting to use EnsureCreated as fallback in development mode.");
            dbContext.Database.EnsureCreated();
        }
        else
        {
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<app.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Factory class for creating ImageAnalysisClient instances
public class ImageAnalysisClientFactory
{
    private readonly string _endpoint;
    private readonly string _apiKey;

    public ImageAnalysisClientFactory(string endpoint, string apiKey)
    {
        _endpoint = endpoint;
        _apiKey = apiKey;
    }

    public ImageAnalysisClient CreateClient()
    {
        return new ImageAnalysisClient(
            new Uri(_endpoint),
            new AzureKeyCredential(_apiKey));
    }
}
