using FacturasClaude.Api.Repositories.Interfaces;
using FacturasClaude.Api.Services.Interfaces;
using FacturasClaude.Models.Entities;
using FacturasClaude.Models.Responses;

namespace FacturasClaude.Api.Services;

public class ClaudeUsageService : IClaudeUsageService
{
    private readonly IClaudeUsageRepository _repository;
    private readonly ILogger<ClaudeUsageService> _logger;

    public ClaudeUsageService(IClaudeUsageRepository pRepository, ILogger<ClaudeUsageService> pLogger)
    {
        _repository = pRepository;
        _logger     = pLogger;
    }

    public async Task RecordAsync(int pDocumentId, AnthropicMessageResponse pResponse, CancellationToken pCancellationToken = default)
    {
        var record = new ClaudeUsageRecord
        {
            DocumentId           = pDocumentId,
            MessageId            = pResponse.Id,
            RequestId            = pResponse.RequestId,
            Model                = pResponse.Model,
            StopReason           = pResponse.StopReason,
            InputTokens          = pResponse.Usage?.InputTokens ?? 0,
            OutputTokens         = pResponse.Usage?.OutputTokens ?? 0,
            CacheCreationTokens  = pResponse.Usage?.CacheCreationInputTokens ?? 0,
            CacheReadTokens      = pResponse.Usage?.CacheReadInputTokens ?? 0,
            ElapsedMs            = (int)pResponse.ElapsedMilliseconds,
            CreatedAt            = DateTime.Now
        };

        await _repository.InsertAsync(record, pCancellationToken);

        _logger.LogInformation(
            "Claude usage recorded: RequestId={RequestId}, Tokens={Input}in/{Output}out, Elapsed={Elapsed}ms",
            record.RequestId, record.InputTokens, record.OutputTokens, record.ElapsedMs);
    }
}
