namespace PriceTracker.Api.Models;

public class StockPrice
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Exchange { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DayHigh { get; set; }
    public decimal DayLow { get; set; }
    public decimal Open { get; set; }
    public decimal PreviousClose { get; set; }
    public decimal ChangePercent { get; set; }
    public long Volume { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
