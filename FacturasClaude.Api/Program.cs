using FacturasClaude.Api.Middleware;
using FacturasClaude.Api.Repositories;
using FacturasClaude.Api.Repositories.Interfaces;
using FacturasClaude.Api.Services;
using FacturasClaude.Api.Services.Interfaces;
using FacturasClaude.Models.Settings;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not configured.");

var anthropicApiKey =
    builder.Configuration["Anthropic:ApiKey"]
    ?? throw new InvalidOperationException("Anthropic ApiKey not configured.");

builder.Services.AddControllers();
builder.Services.AddScoped<IInvoiceExtractionService, InvoiceExtractionService>();
builder.Services.AddScoped<IDocumentRepository>(_ => new DocumentRepository(connectionString));
builder.Services.AddScoped<IInvoiceRepository>(_ => new InvoiceRepository(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AnthropicSettings>(
    builder.Configuration.GetSection("Anthropic"));

builder.Services.AddHttpClient<IAnthropicHttpClient, AnthropicHttpClient>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("https://api.anthropic.com/");
        client.DefaultRequestHeaders.Add("x-api-key", anthropicApiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
    });

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
