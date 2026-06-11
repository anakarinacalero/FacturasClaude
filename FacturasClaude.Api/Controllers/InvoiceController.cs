using FacturasClaude.Api.Controllers.Interfaces;
using FacturasClaude.Api.Repositories.Interfaces;
using FacturasClaude.Api.Services.Interfaces;
using FacturasClaude.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FacturasClaude.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase, IInvoiceController
{
    private readonly IInvoiceExtractionService _extractionService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<InvoiceController> _logger;

    private static readonly HashSet<string> AllowedMediaTypes =
    [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    ];

    public InvoiceController(
        IInvoiceExtractionService pExtractionService,
        IDocumentRepository pDocumentRepository,
        IInvoiceRepository pInvoiceRepository,
        ILogger<InvoiceController> pLogger)
    {
        _extractionService   = pExtractionService;
        _documentRepository  = pDocumentRepository;
        _invoiceRepository   = pInvoiceRepository;
        _logger              = pLogger;
    }

    [HttpPost("extract")]
    public async Task<IActionResult> ExtractAsync(IFormFile pFile, CancellationToken pCancellationToken)
    {
        if (pFile is null || pFile.Length == 0)
            return BadRequest("No file was provided.");

        if (!AllowedMediaTypes.Contains(pFile.ContentType))
            return BadRequest($"File type '{pFile.ContentType}' is not supported.");

        _logger.LogInformation("Processing invoice file: {FileName} ({ContentType}, {Size} bytes)",
            pFile.FileName, pFile.ContentType, pFile.Length);

        var document = new DocumentRecord
        {
            FileName   = pFile.FileName,
            MediaType  = pFile.ContentType,
            FileSize   = pFile.Length,
            UploadedAt = DateTime.UtcNow
        };

        var documentId = await _documentRepository.InsertAsync(document, pCancellationToken);

        await using var stream = pFile.OpenReadStream();
        var invoiceData = await _extractionService.ExtractAsync(stream, pFile.ContentType, documentId, pCancellationToken);

        await _invoiceRepository.InsertAsync(invoiceData, documentId, pCancellationToken);

        return Ok(invoiceData);
    }
}
