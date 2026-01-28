using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PriceTracker.Api.Data;
using PriceTracker.Api.Models;
using System.Text.Json;

namespace PriceTracker.Api.Services;

public class StockService : IStockService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<StockService> _logger;
    private readonly IConfiguration _configuration;

    public StockService(
        AppDbContext context,
        IDistributedCache cache,
        ILogger<StockService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IEnumerable<StockPrice>> GetAllAsync()
    {
        const string cacheKey = "stocks:all";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<IEnumerable<StockPrice>>(cached) ?? [];
        }

        var prices = await _context.StockPrices
            .OrderBy(s => s.Symbol)
            .ToListAsync();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(prices),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                    _configuration.GetValue<int>("Cache:StockPriceExpirationMinutes"))
            });

        return prices;
    }

    public async Task<StockPrice?> GetBySymbolAsync(string symbol)
    {
        var cacheKey = $"stocks:symbol:{symbol.ToUpper()}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<StockPrice>(cached);
        }

        var price = await _context.StockPrices
            .FirstOrDefaultAsync(s => s.Symbol == symbol.ToUpper());

        if (price != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(price),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        _configuration.GetValue<int>("Cache:StockPriceExpirationMinutes"))
                });
        }

        return price;
    }

    public async Task UpdatePricesAsync()
    {
        try
        {
            _logger.LogInformation("Updating stock prices");

            // Mock data for demonstration (in production, use Alpha Vantage or similar API)
            var stocks = new[]
            {
                new StockPrice { Symbol = "AAPL", Name = "Apple Inc.", Exchange = "NASDAQ", Price = 178.50m, DayHigh = 180.25m, DayLow = 177.10m, Open = 179.00m, PreviousClose = 177.80m, ChangePercent = 0.39m, Volume = 52384000 },
                new StockPrice { Symbol = "MSFT", Name = "Microsoft Corporation", Exchange = "NASDAQ", Price = 420.75m, DayHigh = 425.30m, DayLow = 418.50m, Open = 422.00m, PreviousClose = 419.80m, ChangePercent = 0.23m, Volume = 21456000 },
                new StockPrice { Symbol = "GOOGL", Name = "Alphabet Inc.", Exchange = "NASDAQ", Price = 142.65m, DayHigh = 144.20m, DayLow = 141.80m, Open = 143.50m, PreviousClose = 143.10m, ChangePercent = -0.31m, Volume = 18234000 },
                new StockPrice { Symbol = "AMZN", Name = "Amazon.com Inc.", Exchange = "NASDAQ", Price = 178.90m, DayHigh = 180.50m, DayLow = 177.20m, Open = 179.30m, PreviousClose = 178.00m, ChangePercent = 0.51m, Volume = 35678000 },
                new StockPrice { Symbol = "TSLA", Name = "Tesla Inc.", Exchange = "NASDAQ", Price = 248.30m, DayHigh = 252.80m, DayLow = 245.10m, Open = 250.00m, PreviousClose = 246.50m, ChangePercent = 0.73m, Volume = 98765000 }
            };

            foreach (var stock in stocks)
            {
                var existing = await _context.StockPrices.FirstOrDefaultAsync(s => s.Symbol == stock.Symbol);

                if (existing == null)
                {
                    stock.LastUpdated = DateTime.UtcNow;
                    _context.StockPrices.Add(stock);
                }
                else
                {
                    existing.Price = stock.Price;
                    existing.DayHigh = stock.DayHigh;
                    existing.DayLow = stock.DayLow;
                    existing.Open = stock.Open;
                    existing.PreviousClose = stock.PreviousClose;
                    existing.ChangePercent = stock.ChangePercent;
                    existing.Volume = stock.Volume;
                    existing.LastUpdated = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            // Invalidate cache
            await _cache.RemoveAsync("stocks:all");

            _logger.LogInformation("Successfully updated {Count} stock prices", stocks.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock prices");
            throw;
        }
    }
}
