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
        const string mainSql = """
            INSERT INTO tDSVFACextractedData
                (document_id, invoice_number, issue_date, total_amount, currency, supplier, client_address, client_zip_code, description, taxes)
            VALUES
                (@document_id, @invoice_number, @issue_date, @total_amount, @currency, @supplier, @client_address, @client_zip_code, @description, @taxes);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        const string conceptSql = """
            INSERT INTO tDSVFACinvoiceConcept
                (extracted_data_id, description, quantity, unit_of_measure, unit_price, subtotal)
            VALUES
                (@extracted_data_id, @description, @quantity, @unit_of_measure, @unit_price, @subtotal);
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(pCancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(pCancellationToken);

        var mainCommand = new CommandDefinition(mainSql, new
        {
            document_id = pDocumentId,
            invoice_number = pInvoiceData.InvoiceNumber,
            issue_date = pInvoiceData.IssueDate.HasValue
                ? (object)pInvoiceData.IssueDate.Value.Date
                : DBNull.Value,
            total_amount = pInvoiceData.TotalAmount,
            currency = pInvoiceData.Currency,
            supplier = pInvoiceData.Supplier,
            client_address = pInvoiceData.ClientAddress,
            client_zip_code = pInvoiceData.ClientZipCode,
            description = pInvoiceData.Description,
            taxes = pInvoiceData.Taxes
        }, transaction: transaction, cancellationToken: pCancellationToken);

        var extractedDataId = await connection.ExecuteScalarAsync<int>(mainCommand);

        if (pInvoiceData.Concepts is { Count: > 0 })
        {
            var conceptParameters = pInvoiceData.Concepts.Select(c => new
            {
                extracted_data_id = extractedDataId,
                description = c.Description,
                quantity = c.Quantity,
                unit_of_measure = c.UnitOfMeasure,
                unit_price = c.UnitPrice,
                subtotal = c.Subtotal
            });

            var conceptCommand = new CommandDefinition(
                conceptSql,
                conceptParameters,
                transaction: transaction,
                cancellationToken: pCancellationToken);

            await connection.ExecuteAsync(conceptCommand);
        }

        await transaction.CommitAsync(pCancellationToken);
    }

    public async Task<InvoiceData?> GetByDocumentIdAsync(int pDocumentId, CancellationToken pCancellationToken = default)
    {
        const string sql = """
            DECLARE @extracted_data_id INT =
                (SELECT TOP 1 id FROM tDSVFACextractedData WHERE document_id = @document_id);

            SELECT invoice_number  AS InvoiceNumber,
                   issue_date      AS IssueDate,
                   total_amount    AS TotalAmount,
                   currency        AS Currency,
                   supplier        AS Supplier,
                   client_address  AS ClientAddress,
                   client_zip_code AS ClientZipCode,
                   description     AS Description,
                   taxes           AS Taxes
            FROM tDSVFACextractedData
            WHERE id = @extracted_data_id;

            SELECT description    AS Description,
                   quantity       AS Quantity,
                   unit_of_measure AS UnitOfMeasure,
                   unit_price     AS UnitPrice,
                   subtotal       AS Subtotal
            FROM tDSVFACinvoiceConcept
            WHERE extracted_data_id = @extracted_data_id;
            """;

        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(sql, new { document_id = pDocumentId }, cancellationToken: pCancellationToken);

        using var multi = await connection.QueryMultipleAsync(command);
        var data = await multi.ReadFirstOrDefaultAsync<InvoiceData>();
        if (data is null)
            return null;

        data.Concepts = (await multi.ReadAsync<InvoiceConcept>()).ToList();
        return data;
    }
}
