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

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
