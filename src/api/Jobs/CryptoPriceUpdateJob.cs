using PriceTracker.Api.Services;

namespace PriceTracker.Api.Jobs;

public class CryptoPriceUpdateJob
{
    private readonly ICryptoService _cryptoService;
    private readonly ILogger<CryptoPriceUpdateJob> _logger;

    public CryptoPriceUpdateJob(
        ICryptoService cryptoService,
        ILogger<CryptoPriceUpdateJob> logger)
    {
        _cryptoService = cryptoService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting cryptocurrency price update job");

        try
        {
            await _cryptoService.UpdatePricesAsync();
            _logger.LogInformation("Cryptocurrency price update job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cryptocurrency price update job");
            throw;
        }
    }
}
