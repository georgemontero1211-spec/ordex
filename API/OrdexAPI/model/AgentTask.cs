

public class AgentTask
{
    public Guid Id { get; set; }
    public required string AgentId { get; set; }
    public required string Type { get; set; }
    public string? Payload { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Output { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}