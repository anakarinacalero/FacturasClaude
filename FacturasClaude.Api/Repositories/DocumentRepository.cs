using Dapper;
using FacturasClaude.Api.Repositories.Interfaces;
using FacturasClaude.Models.Entities;
using Microsoft.Data.SqlClient;

namespace FacturasClaude.Api.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly string _connectionString;

    public DocumentRepository(string pConnectionString)
    {
        _connectionString = pConnectionString;
    }

    public async Task<int> InsertAsync(DocumentRecord pDocument, CancellationToken pCancellationToken = default)
    {
        const string sql = """
            INSERT INTO tDSVFACdocument (file_name, media_type, file_size, uploaded_at)
            VALUES (@file_name, @media_type, @file_size, @uploaded_at);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(sql, new
        {
            file_name  = pDocument.FileName,
            media_type = pDocument.MediaType,
            file_size  = pDocument.FileSize,
            uploaded_at = pDocument.UploadedAt
        }, cancellationToken: pCancellationToken);

        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<DocumentRecord?> GetByIdAsync(int pId, CancellationToken pCancellationToken = default)
    {
        const string sql = """
            SELECT
                    id          AS Id,
                    file_name   AS FileName,
                    media_type  AS MediaType,
                    file_size   AS FileSize,
                    uploaded_at AS UploadedAt
            FROM tDSVFACdocument
            WHERE id = @id;
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(sql, new { id = pId }, cancellationToken: pCancellationToken);

        return await connection.QueryFirstOrDefaultAsync<DocumentRecord>(command);
    }
}
