using Microsoft.EntityFrameworkCore;
using PriceTracker.Api.Data;
using PriceTracker.Api.Models;

namespace PriceTracker.Api.Services;

public class AlertService : IAlertService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        AppDbContext context,
        INotificationService notificationService,
        ILogger<AlertService> logger)
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<IEnumerable<Alert>> GetAllAsync()
    {
        return await _context.Alerts
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Alert?> GetByIdAsync(int id)
    {
        return await _context.Alerts.FindAsync(id);
    }

    public async Task<Alert> CreateAsync(CreateAlertRequest request)
    {
        var alert = new Alert
        {
            AssetType = request.AssetType.ToLower(),
            Symbol = request.Symbol.ToUpper(),
            Condition = request.Condition.ToLower(),
            TargetPrice = request.TargetPrice,
            WebhookUrl = request.WebhookUrl,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created alert {AlertId} for {Symbol} {Condition} {Price}",
            alert.Id, alert.Symbol, alert.Condition, alert.TargetPrice);

        return alert;
    }

    public async Task DeleteAsync(int id)
    {
        var alert = await _context.Alerts.FindAsync(id);
        if (alert != null)
        {
            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted alert {AlertId}", id);
        }
    }

    public async Task CheckAlertsAsync()
    {
        var activeAlerts = await _context.Alerts
            .Where(a => a.IsActive && !a.IsTriggered)
            .ToListAsync();

        _logger.LogInformation("Checking {Count} active alerts", activeAlerts.Count);

        foreach (var alert in activeAlerts)
        {
            decimal? currentPrice = null;

            try
            {
                currentPrice = alert.AssetType switch
                {
                    "crypto" => (await _context.CryptoPrices.FirstOrDefaultAsync(c => c.Symbol == alert.Symbol))?.PriceUsd,
                    "stock" => (await _context.StockPrices.FirstOrDefaultAsync(s => s.Symbol == alert.Symbol))?.Price,
                    "forex" => (await _context.ForexRates.FirstOrDefaultAsync(f => f.BaseCurrency == "USD" && f.TargetCurrency == alert.Symbol))?.Rate,
                    _ => null
                };

                if (currentPrice.HasValue)
                {
                    bool shouldTrigger = alert.Condition switch
                    {
                        "above" => currentPrice.Value >= alert.TargetPrice,
                        "below" => currentPrice.Value <= alert.TargetPrice,
                        _ => false
                    };

                    if (shouldTrigger)
                    {
                        alert.IsTriggered = true;
                        alert.TriggeredAt = DateTime.UtcNow;
                        alert.IsActive = false;

                        await _notificationService.SendAlertNotificationAsync(alert, currentPrice.Value);

                        _logger.LogInformation("Alert {AlertId} triggered: {Symbol} is {Condition} {TargetPrice} (current: {CurrentPrice})",
                            alert.Id, alert.Symbol, alert.Condition, alert.TargetPrice, currentPrice.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking alert {AlertId}", alert.Id);
            }
        }

        await _context.SaveChangesAsync();
    }
}
