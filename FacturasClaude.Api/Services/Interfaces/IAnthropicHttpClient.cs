using FacturasClaude.Models.Responses;

namespace FacturasClaude.Api.Services.Interfaces;

public interface IAnthropicHttpClient
{
    Task<AnthropicMessageResponse> SendMessageAsync(object[] pMessages, CancellationToken pCancellationToken = default);
}
