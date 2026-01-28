using PriceTracker.Api.Models;

namespace PriceTracker.Api.Services;

public interface ICryptoService
{
    Task<IEnumerable<CryptoPrice>> GetAllAsync();
    Task<CryptoPrice?> GetBySymbolAsync(string symbol);
    Task<IEnumerable<CryptoPrice>> GetTopAsync(int count = 10);
    Task UpdatePricesAsync();
}
