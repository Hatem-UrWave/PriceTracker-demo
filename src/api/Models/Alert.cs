namespace PriceTracker.Api.Models;

public class Alert
{
    public int Id { get; set; }
    public string AssetType { get; set; } = string.Empty; // crypto, stock, forex
    public string Symbol { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty; // above, below
    public decimal TargetPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTriggered { get; set; } = false;
    public DateTime? TriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? WebhookUrl { get; set; }
    public string? Email { get; set; }
}

public record CreateAlertRequest(
    string AssetType,
    string Symbol,
    string Condition,
    decimal TargetPrice,
    string? WebhookUrl = null,
    string? Email = null
);
