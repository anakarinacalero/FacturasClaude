using Microsoft.AspNetCore.Mvc;

namespace FacturasClaude.Api.Controllers.Interfaces;

public interface IInvoiceController
{
    Task<IActionResult> ExtractAsync(IFormFile pFile, CancellationToken pCancellationToken);
}
