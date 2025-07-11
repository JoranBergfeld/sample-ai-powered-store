using app.Components;
using app.Data;
using app.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("SampleStoreDb"));

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
    // Fallback to a mock service for development
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: "gpt-4",
        endpoint: "https://mock-endpoint.openai.azure.com/",
        apiKey: "mock-key");
}

// Register services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AIService>();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
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
