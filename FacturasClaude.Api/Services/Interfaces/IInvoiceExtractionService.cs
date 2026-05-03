using FacturasClaude.Models.Entities;

namespace FacturasClaude.Api.Services.Interfaces;

public interface IInvoiceExtractionService
{
    Task<InvoiceData> ExtractAsync(Stream pFileStream, string pMediaType, CancellationToken pCancellationToken = default);
}
