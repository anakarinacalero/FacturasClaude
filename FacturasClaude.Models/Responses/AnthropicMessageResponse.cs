using System.Text.Json.Serialization;
using FacturasClaude.Models.DTOs;

namespace FacturasClaude.Models.Responses;

public class AnthropicMessageResponse
{
    public string Id { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; set; }

    public List<AnthropicContentBlock> Content { get; set; } = new();
    public AnthropicUsage? Usage { get; set; }

    [JsonIgnore]
    public string? RequestId { get; set; }

    [JsonIgnore]
    public long ElapsedMilliseconds { get; set; }
}
