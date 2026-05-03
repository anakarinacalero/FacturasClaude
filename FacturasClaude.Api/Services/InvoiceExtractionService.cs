using System.Text.Json;
using System.Text.RegularExpressions;
using FacturasClaude.Api.Services.Interfaces;
using FacturasClaude.Models.Entities;
using FacturasClaude.Models.DTOs;
using FacturasClaude.Models.Responses;

namespace FacturasClaude.Api.Services;

public class InvoiceExtractionService : IInvoiceExtractionService
{
    private readonly AnthropicHttpClient _anthropicClient;
    private readonly ILogger<InvoiceExtractionService> _logger;

    private const string ExtractionPrompt = """
        Analyze this invoice document and extract the following information.
        Return ONLY a valid JSON object with exactly these fields (use null for missing values):
        {
          "invoice_number": "string or null",
          "issue_date": "YYYY-MM-DD (required)",
          "total_amount": number or null,
          "currency": "ISO 4217 code or null",
          "supplier": "string or null",
          "description": "string or null"
        }
        """;

    public InvoiceExtractionService(
        AnthropicHttpClient pAnthropicClient,
        ILogger<InvoiceExtractionService> pLogger)
    {
        _anthropicClient = pAnthropicClient;
        _logger = pLogger;
    }

    public async Task<InvoiceData> ExtractAsync(
        Stream pFileStream,string pMediaType,CancellationToken pCancellationToken = default)
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

        var responseText = await _anthropicClient.SendMessageAsync(messages, pCancellationToken);

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
