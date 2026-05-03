using System.Text.Json.Serialization;

namespace FacturasClaude.Models.Entities;

public class InvoiceConcept
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    [JsonPropertyName("unit_of_measure")]
    public string? UnitOfMeasure { get; set; }

    [JsonPropertyName("unit_price")]
    public decimal? UnitPrice { get; set; }

    [JsonPropertyName("subtotal")]
    public decimal? Subtotal { get; set; }
}
