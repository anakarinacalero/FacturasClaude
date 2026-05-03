using System.Text.Json.Serialization;
using FacturasClaude.Models.DTOs;

namespace FacturasClaude.Models.Responses;

public class AnthropicMessageResponse
{
    
    public List<AnthropicContentBlock> Content { get; set; } = new List<AnthropicContentBlock>();
}
