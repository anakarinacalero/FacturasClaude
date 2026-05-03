using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using FacturasClaude.Api.Services.Interfaces;
using FacturasClaude.Models.Responses;
using FacturasClaude.Models.Settings;
using Microsoft.Extensions.Options;

namespace FacturasClaude.Api.Services;

public class AnthropicHttpClient : IAnthropicHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AnthropicSettings _settings;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AnthropicHttpClient(HttpClient pHttpClient,IOptions<AnthropicSettings> pSettings)
    {
        _httpClient = pHttpClient;
        _settings = pSettings.Value;
    }

    public async Task<AnthropicMessageResponse> SendMessageAsync(object[] pMessages, CancellationToken pCancellationToken = default)
    {
        var request = new
        {
            model = _settings.Model,
            max_tokens = 1024,
            messages = pMessages
        };

        var stopwatch = Stopwatch.StartNew();

        var response = await _httpClient.PostAsJsonAsync(
            "v1/messages",
            request,
            SerializerOptions,
            pCancellationToken);

        stopwatch.Stop();

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AnthropicMessageResponse>(
            SerializerOptions,
            pCancellationToken) ?? new AnthropicMessageResponse();

        result.RequestId = response.Headers.TryGetValues("request-id", out var values)
            ? values.FirstOrDefault()
            : null;

        result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        return result;
    }
}
