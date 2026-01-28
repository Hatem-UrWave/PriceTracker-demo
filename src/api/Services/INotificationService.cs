using PriceTracker.Api.Models;

namespace PriceTracker.Api.Services;

public interface INotificationService
{
    Task SendAlertNotificationAsync(Alert alert, decimal currentPrice);
}
