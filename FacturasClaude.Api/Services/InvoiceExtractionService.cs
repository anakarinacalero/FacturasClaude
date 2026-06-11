using System.Text.Json;
using System.Text.RegularExpressions;
using FacturasClaude.Api.Services.Interfaces;
using FacturasClaude.Models.DTOs;
using FacturasClaude.Models.Entities;
using FacturasClaude.Models.Responses;

namespace FacturasClaude.Api.Services;

public class InvoiceExtractionService : IInvoiceExtractionService
{
    private readonly IAnthropicHttpClient _anthropicClient;
    private readonly IClaudeUsageService _claudeUsageService;
    private readonly ILogger<InvoiceExtractionService> _logger;

    private const string ExtractionPrompt = """
        Analiza esta factura y extrae la siguiente información.
        Devuelve ÚNICAMENTE un objeto JSON válido con exactamente estos campos (usa null para valores faltantes):
        {
          "invoice_number": "string or null",
          "issue_date": "YYYY-MM-DD (requerido)",
          "total_amount": number or null,
          "currency": "ISO 4217 code or null",
          "supplier": "string or null",
          "client_address": "string or null (dirección completa del cliente/destinatario)",
          "client_zip_code": "string or null (código postal del cliente/destinatario)",
          "description": "string or null",
          "taxes": number or null (importe total de impuestos en la factura),
          "concepts": [
            {
              "description": "string",
              "quantity": number or null,
              "unit_of_measure": "string or null (e.g. pcs, kg, hrs)",
              "unit_price": number or null,
              "subtotal": number or null
            }
          ]
        }
        Si la factura no tiene líneas de detalle, devuelve concepts como un arreglo vacío.
        """;

    public InvoiceExtractionService(IAnthropicHttpClient pAnthropicClient,IClaudeUsageService pClaudeUsageService,ILogger<InvoiceExtractionService> pLogger)
    {
        _anthropicClient    = pAnthropicClient;
        _claudeUsageService = pClaudeUsageService;
        _logger             = pLogger;
    }

    public async Task<InvoiceData> ExtractAsync(
        Stream pFileStream, string pMediaType, int pDocumentId, CancellationToken pCancellationToken = default)
    {
        var base64Data = await ToBase64Async(pFileStream, pCancellationToken);

        var messages = new object[]
        {
            new
            {
                role = "user",
                content = new object[]
                {
                    BuildContentBlock(pMediaType, base64Data),
                    new { type = "text", text = ExtractionPrompt }
                }
            }
        };

        var response = await _anthropicClient.SendMessageAsync(messages, pCancellationToken);

        await _claudeUsageService.RecordAsync(pDocumentId, response, pCancellationToken);

        var responseText = response.Content
            .FirstOrDefault(c => c.Type == "text")?.Text
            ?? string.Empty;

        return ParseInvoiceData(responseText);
    }

    private static async Task<string> ToBase64Async(Stream pStream, CancellationToken pCancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await pStream.CopyToAsync(memoryStream, pCancellationToken);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private static object BuildContentBlock(string pMediaType, string pBase64Data)
    {
        var blockType = pMediaType == "application/pdf" ? "document" : "image";

        return new
        {
            type = blockType,
            source = new
            {
                type = "base64",
                media_type = pMediaType,
                data = pBase64Data
            }
        };
    }

    private InvoiceData ParseInvoiceData(string pResponseText)
    {
        var json = ExtractJson(pResponseText);

        try
        {
            return JsonSerializer.Deserialize<InvoiceData>(json) ?? new InvoiceData();
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Claude response into InvoiceData");
            return new InvoiceData();
        }
    }

    private static string ExtractJson(string pText)
    {
        var fenceMatch = Regex.Match(pText, @"```(?:json)?\s*([\s\S]*?)\s*```");
        if (fenceMatch.Success)
            return fenceMatch.Groups[1].Value.Trim();

        var start = pText.IndexOf('{');
        var end = pText.LastIndexOf('}');
        if (start >= 0 && end > start)
            return pText[start..(end + 1)];

        return pText.Trim();
    }
}
