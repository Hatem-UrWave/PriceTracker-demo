using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PriceTracker.Api.Data;
using PriceTracker.Api.Models;
using System.Text.Json;

namespace PriceTracker.Api.Services;

public class CryptoService : ICryptoService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CryptoService> _logger;
    private readonly IConfiguration _configuration;

    public CryptoService(
        AppDbContext context,
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        ILogger<CryptoService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IEnumerable<CryptoPrice>> GetAllAsync()
    {
        const string cacheKey = "crypto:all";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<IEnumerable<CryptoPrice>>(cached) ?? [];
        }

        var prices = await _context.CryptoPrices
            .OrderByDescending(c => c.MarketCapUsd)
            .ToListAsync();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(prices),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                    _configuration.GetValue<int>("Cache:CryptoPriceExpirationMinutes"))
            });

        return prices;
    }

    public async Task<CryptoPrice?> GetBySymbolAsync(string symbol)
    {
        var cacheKey = $"crypto:symbol:{symbol.ToUpper()}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<CryptoPrice>(cached);
        }

        var price = await _context.CryptoPrices
            .FirstOrDefaultAsync(c => c.Symbol == symbol.ToUpper());

        if (price != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(price),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        _configuration.GetValue<int>("Cache:CryptoPriceExpirationMinutes"))
                });
        }

        return price;
    }

    public async Task<IEnumerable<CryptoPrice>> GetTopAsync(int count = 10)
    {
        var cacheKey = $"crypto:top:{count}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<IEnumerable<CryptoPrice>>(cached) ?? [];
        }

        var prices = await _context.CryptoPrices
            .OrderByDescending(c => c.MarketCapUsd)
            .Take(count)
            .ToListAsync();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(prices),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                    _configuration.GetValue<int>("Cache:CryptoPriceExpirationMinutes"))
            });

        return prices;
    }

    public async Task UpdatePricesAsync()
    {
        try
        {
            _logger.LogInformation("Updating cryptocurrency prices from CoinGecko API");

            var client = _httpClientFactory.CreateClient("CoinGecko");
            var symbols = new[] { "bitcoin", "ethereum", "binancecoin", "cardano", "solana", "ripple", "polkadot", "dogecoin", "avalanche-2", "polygon" };

            var response = await client.GetAsync($"simple/price?ids={string.Join(",", symbols)}&vs_currencies=usd,eur&include_market_cap=true&include_24hr_vol=true&include_24hr_change=true");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(content);

            var nameMap = new Dictionary<string, (string Symbol, string Name)>
            {
                ["bitcoin"] = ("BTC", "Bitcoin"),
                ["ethereum"] = ("ETH", "Ethereum"),
                ["binancecoin"] = ("BNB", "Binance Coin"),
                ["cardano"] = ("ADA", "Cardano"),
                ["solana"] = ("SOL", "Solana"),
                ["ripple"] = ("XRP", "Ripple"),
                ["polkadot"] = ("DOT", "Polkadot"),
                ["dogecoin"] = ("DOGE", "Dogecoin"),
                ["avalanche-2"] = ("AVAX", "Avalanche"),
                ["polygon"] = ("MATIC", "Polygon")
            };

            foreach (var symbol in symbols)
            {
                if (data.TryGetProperty(symbol, out var coinData))
                {
                    var (sym, name) = nameMap[symbol];
                    var existing = await _context.CryptoPrices.FirstOrDefaultAsync(c => c.Symbol == sym);

                    var price = new CryptoPrice
                    {
                        Symbol = sym,
                        Name = name,
                        PriceUsd = coinData.GetProperty("usd").GetDecimal(),
                        PriceEur = coinData.GetProperty("eur").GetDecimal(),
                        MarketCapUsd = coinData.GetProperty("usd_market_cap").GetDecimal(),
                        Volume24hUsd = coinData.GetProperty("usd_24h_vol").GetDecimal(),
                        ChangePercent24h = coinData.GetProperty("usd_24h_change").GetDecimal(),
                        LastUpdated = DateTime.UtcNow
                    };

                    if (existing == null)
                    {
                        _context.CryptoPrices.Add(price);
                    }
                    else
                    {
                        existing.PriceUsd = price.PriceUsd;
                        existing.PriceEur = price.PriceEur;
                        existing.MarketCapUsd = price.MarketCapUsd;
                        existing.Volume24hUsd = price.Volume24hUsd;
                        existing.ChangePercent24h = price.ChangePercent24h;
                        existing.LastUpdated = price.LastUpdated;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Invalidate cache
            await _cache.RemoveAsync("crypto:all");
            for (int i = 1; i <= 20; i++)
            {
                await _cache.RemoveAsync($"crypto:top:{i}");
            }

            _logger.LogInformation("Successfully updated {Count} cryptocurrency prices", symbols.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cryptocurrency prices");
            throw;
        }
    }
}
