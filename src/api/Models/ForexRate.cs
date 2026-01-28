namespace PriceTracker.Api.Models;

public class ForexRate
{
    public int Id { get; set; }
    public string BaseCurrency { get; set; } = string.Empty;
    public string TargetCurrency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
