namespace FacturasClaude.Models.Entities;

public class ClaudeUsageRecord
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string? MessageId { get; set; }
    public string? RequestId { get; set; }
    public string Model { get; set; } = string.Empty;
    public string? StopReason { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int CacheCreationTokens { get; set; }
    public int CacheReadTokens { get; set; }
    public int ElapsedMs { get; set; }
    public DateTime CreatedAt { get; set; }
}
