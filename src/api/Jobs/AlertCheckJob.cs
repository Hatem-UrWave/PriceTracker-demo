using PriceTracker.Api.Services;

namespace PriceTracker.Api.Jobs;

public class AlertCheckJob
{
    private readonly IAlertService _alertService;
    private readonly ILogger<AlertCheckJob> _logger;

    public AlertCheckJob(
        IAlertService alertService,
        ILogger<AlertCheckJob> logger)
    {
        _alertService = alertService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting alert check job");

        try
        {
            await _alertService.CheckAlertsAsync();
            _logger.LogInformation("Alert check job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in alert check job");
            throw;
        }
    }
}
