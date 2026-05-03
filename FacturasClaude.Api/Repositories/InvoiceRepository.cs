using Dapper;
using FacturasClaude.Api.Repositories.Interfaces;
using FacturasClaude.Models.Entities;
using Microsoft.Data.SqlClient;

namespace FacturasClaude.Api.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly string _connectionString;

    public InvoiceRepository(string pConnectionString)
    {
        _connectionString = pConnectionString;
    }

    public async Task InsertAsync(InvoiceData pInvoiceData, int pDocumentId, CancellationToken pCancellationToken = default)
    {
        const string sql = """
            INSERT INTO tDSVFACextractedData
                (document_id, invoice_number, issue_date, total_amount, currency, supplier, description)
            VALUES
                (@document_id, @invoice_number, @issue_date, @total_amount, @currency, @supplier, @description);
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(sql, new
        {
            document_id = pDocumentId,
            invoice_number = pInvoiceData.InvoiceNumber,
            issue_date = pInvoiceData.IssueDate.HasValue
                ? (object)pInvoiceData.IssueDate.Value.Date
                : DBNull.Value,
            total_amount = pInvoiceData.TotalAmount,
            currency = pInvoiceData.Currency,
            supplier = pInvoiceData.Supplier,
            description = pInvoiceData.Description,
        }, cancellationToken: pCancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task<InvoiceData?> GetByDocumentIdAsync(int pDocumentId, CancellationToken pCancellationToken = default)
    {
        const string sql = """
            SELECT invoice_number AS InvoiceNumber,
            issue_date     AS IssueDate,
            total_amount   AS TotalAmount,
            currency       AS Currency,
            supplier       AS Supplier,
            description    AS Description
            FROM tDSVFACextractedData
            WHERE document_id = @document_id;
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(sql, new { document_id = pDocumentId }, cancellationToken: pCancellationToken);

        return await connection.QueryFirstOrDefaultAsync<InvoiceData>(command);
    }
}
