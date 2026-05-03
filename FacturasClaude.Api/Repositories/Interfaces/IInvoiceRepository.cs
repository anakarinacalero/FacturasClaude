using FacturasClaude.Models.Entities;

namespace FacturasClaude.Api.Repositories.Interfaces;

public interface IInvoiceRepository
{
    Task InsertAsync(InvoiceData pInvoiceData, int pDocumentId, CancellationToken pCancellationToken = default);
    Task<InvoiceData?> GetByDocumentIdAsync(int pDocumentId, CancellationToken pCancellationToken = default);
}
