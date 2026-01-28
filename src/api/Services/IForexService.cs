using PriceTracker.Api.Models;

namespace PriceTracker.Api.Services;

public interface IForexService
{
    Task<IEnumerable<ForexRate>> GetAllAsync();
    Task<ForexRate?> GetRateAsync(string baseCurrency, string targetCurrency);
    Task UpdateRatesAsync();
}
