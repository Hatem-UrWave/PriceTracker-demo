using PriceTracker.Api.Services;

namespace PriceTracker.Api.Jobs;

public class StockPriceUpdateJob
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockPriceUpdateJob> _logger;

    public StockPriceUpdateJob(
        IStockService stockService,
        ILogger<StockPriceUpdateJob> logger)
    {
        _stockService = stockService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting stock price update job");

        try
        {
            await _stockService.UpdatePricesAsync();
            _logger.LogInformation("Stock price update job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in stock price update job");
            throw;
        }
    }
}
