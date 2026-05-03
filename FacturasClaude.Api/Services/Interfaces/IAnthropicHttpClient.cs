namespace FacturasClaude.Api.Services.Interfaces;

public interface IAnthropicHttpClient
{
    Task<string> SendMessageAsync(object[] pMessages,CancellationToken pCancellationToken = default);
}
