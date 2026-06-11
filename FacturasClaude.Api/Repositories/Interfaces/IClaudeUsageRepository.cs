using FacturasClaude.Models.Entities;

namespace FacturasClaude.Api.Repositories.Interfaces;

public interface IClaudeUsageRepository
{
    Task InsertAsync(ClaudeUsageRecord pRecord, CancellationToken pCancellationToken = default);
}
