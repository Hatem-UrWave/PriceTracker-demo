using PriceTracker.Api.Services;

namespace PriceTracker.Api.Jobs;

public class ForexRateUpdateJob
{
    private readonly IForexService _forexService;
    private readonly ILogger<ForexRateUpdateJob> _logger;

    public ForexRateUpdateJob(
        IForexService forexService,
        ILogger<ForexRateUpdateJob> logger)
    {
        _forexService = forexService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting forex rate update job");

        try
        {
            await _forexService.UpdateRatesAsync();
            _logger.LogInformation("Forex rate update job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in forex rate update job");
            throw;
        }
    }
}
