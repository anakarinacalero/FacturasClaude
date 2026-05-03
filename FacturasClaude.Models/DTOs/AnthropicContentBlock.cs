using System.Text.Json.Serialization;

namespace FacturasClaude.Models.DTOs;

public class AnthropicContentBlock
{
   
    public string Type { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public string? Id { get; set; }

    public string? Name { get; set; }

    public object? Input { get; set; }
}
