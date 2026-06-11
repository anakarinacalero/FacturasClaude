using Dapper;
using FacturasClaude.Api.Repositories.Interfaces;
using FacturasClaude.Models.Entities;
using Microsoft.Data.SqlClient;

namespace FacturasClaude.Api.Repositories;

public class ClaudeUsageRepository : IClaudeUsageRepository
{
    private readonly string _connectionString;

    public ClaudeUsageRepository(string pConnectionString)
    {
        _connectionString = pConnectionString;
    }

    public async Task InsertAsync(ClaudeUsageRecord pRecord, CancellationToken pCancellationToken = default)
    {
        const string sql = """
            INSERT INTO tDSVFACclaudeUsage
                (document_id, message_id, request_id, model, stop_reason,
                 input_tokens, output_tokens, cache_creation_tokens, cache_read_tokens,
                 elapsed_ms, created_at)
            VALUES
                (@document_id, @message_id, @request_id, @model, @stop_reason,
                 @input_tokens, @output_tokens, @cache_creation_tokens, @cache_read_tokens,
                 @elapsed_ms, @created_at);
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(sql, new
        {
            document_id            = pRecord.DocumentId,
            message_id             = pRecord.MessageId,
            request_id             = pRecord.RequestId,
            model                  = pRecord.Model,
            stop_reason            = pRecord.StopReason,
            input_tokens           = pRecord.InputTokens,
            output_tokens          = pRecord.OutputTokens,
            cache_creation_tokens  = pRecord.CacheCreationTokens,
            cache_read_tokens      = pRecord.CacheReadTokens,
            elapsed_ms             = pRecord.ElapsedMs,
            created_at             = pRecord.CreatedAt
        }, cancellationToken: pCancellationToken);

        await connection.ExecuteAsync(command);
    }
}
