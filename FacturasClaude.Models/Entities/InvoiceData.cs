using System.Text.Json.Serialization;

namespace FacturasClaude.Models.Entities;

public class InvoiceData
{
    [JsonPropertyName("invoice_number")]
    public string? InvoiceNumber { get; set; }

    [JsonPropertyName("issue_date")]
    public DateTime? IssueDate { get; set; }

    [JsonPropertyName("total_amount")]
    public decimal? TotalAmount { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("supplier")]
    public string? Supplier { get; set; }

    [JsonPropertyName("client_address")]
    public string? ClientAddress { get; set; }

    [JsonPropertyName("client_zip_code")]
    public string? ClientZipCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("taxes")]
    public decimal? Taxes { get; set; }

    [JsonPropertyName("concepts")]
    public List<InvoiceConcept>? Concepts { get; set; }
}
