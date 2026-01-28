namespace PriceTracker.Api.Models;

public class CryptoPrice
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal PriceUsd { get; set; }
    public decimal PriceEur { get; set; }
    public decimal MarketCapUsd { get; set; }
    public decimal Volume24hUsd { get; set; }
    public decimal ChangePercent24h { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
