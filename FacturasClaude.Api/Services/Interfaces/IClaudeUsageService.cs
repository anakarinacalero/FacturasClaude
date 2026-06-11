using FacturasClaude.Models.Responses;

namespace FacturasClaude.Api.Services.Interfaces;

public interface IClaudeUsageService
{
    Task RecordAsync(int pDocumentId, AnthropicMessageResponse pResponse, CancellationToken pCancellationToken = default);
}
