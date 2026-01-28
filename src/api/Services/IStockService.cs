using PriceTracker.Api.Models;

namespace PriceTracker.Api.Services;

public interface IStockService
{
    Task<IEnumerable<StockPrice>> GetAllAsync();
    Task<StockPrice?> GetBySymbolAsync(string symbol);
    Task UpdatePricesAsync();
}
