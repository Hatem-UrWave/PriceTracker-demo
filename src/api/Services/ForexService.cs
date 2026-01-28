using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PriceTracker.Api.Data;
using PriceTracker.Api.Models;
using System.Text.Json;

namespace PriceTracker.Api.Services;

public class ForexService : IForexService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ForexService> _logger;
    private readonly IConfiguration _configuration;

    public ForexService(
        AppDbContext context,
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        ILogger<ForexService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IEnumerable<ForexRate>> GetAllAsync()
    {
        const string cacheKey = "forex:all";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<IEnumerable<ForexRate>>(cached) ?? [];
        }

        var rates = await _context.ForexRates
            .Where(r => r.BaseCurrency == "USD")
            .OrderBy(r => r.TargetCurrency)
            .ToListAsync();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(rates),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                    _configuration.GetValue<int>("Cache:ForexRateExpirationMinutes"))
            });

        return rates;
    }

    public async Task<ForexRate?> GetRateAsync(string baseCurrency, string targetCurrency)
    {
        var cacheKey = $"forex:{baseCurrency}:{targetCurrency}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<ForexRate>(cached);
        }

        var rate = await _context.ForexRates
            .FirstOrDefaultAsync(r =>
                r.BaseCurrency == baseCurrency.ToUpper() &&
                r.TargetCurrency == targetCurrency.ToUpper());

        if (rate != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(rate),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        _configuration.GetValue<int>("Cache:ForexRateExpirationMinutes"))
                });
        }

        return rate;
    }

    public async Task UpdateRatesAsync()
    {
        try
        {
            _logger.LogInformation("Updating forex rates from ExchangeRate API");

            var client = _httpClientFactory.CreateClient("ExchangeRate");
            var response = await client.GetAsync("latest/USD");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(content);

            var rates = data.GetProperty("rates");

            var targetCurrencies = new[] { "EUR", "GBP", "JPY", "CHF", "CAD", "AUD", "NZD", "CNY", "INR", "BRL" };

            foreach (var target in targetCurrencies)
            {
                if (rates.TryGetProperty(target, out var rateValue))
                {
                    var existing = await _context.ForexRates
                        .FirstOrDefaultAsync(r => r.BaseCurrency == "USD" && r.TargetCurrency == target);

                    var forexRate = new ForexRate
                    {
                        BaseCurrency = "USD",
                        TargetCurrency = target,
                        Rate = rateValue.GetDecimal(),
                        LastUpdated = DateTime.UtcNow
                    };

                    if (existing == null)
                    {
                        _context.ForexRates.Add(forexRate);
                    }
                    else
                    {
                        existing.Rate = forexRate.Rate;
                        existing.LastUpdated = forexRate.LastUpdated;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Invalidate cache
            await _cache.RemoveAsync("forex:all");

            _logger.LogInformation("Successfully updated forex rates for {Count} currencies", targetCurrencies.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating forex rates");
            throw;
        }
    }
}
