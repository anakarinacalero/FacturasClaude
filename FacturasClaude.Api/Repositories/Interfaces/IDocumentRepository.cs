using FacturasClaude.Models.Entities;

namespace FacturasClaude.Api.Repositories.Interfaces;

public interface IDocumentRepository
{
    Task<int> InsertAsync(DocumentRecord pDocument, CancellationToken pCancellationToken = default);
    Task<DocumentRecord?> GetByIdAsync(int pId, CancellationToken pCancellationToken = default);
}
