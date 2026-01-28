using PriceTracker.Api.Models;
using System.Text.Json;
using System.Text;

namespace PriceTracker.Api.Services;

public class NotificationService : INotificationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHttpClientFactory httpClientFactory,
        ILogger<NotificationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SendAlertNotificationAsync(Alert alert, decimal currentPrice)
    {
        try
        {
            if (!string.IsNullOrEmpty(alert.WebhookUrl))
            {
                await SendWebhookNotificationAsync(alert, currentPrice);
            }

            if (!string.IsNullOrEmpty(alert.Email))
            {
                await SendEmailNotificationAsync(alert, currentPrice);
            }

            _logger.LogInformation("Sent notifications for alert {AlertId}", alert.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notifications for alert {AlertId}", alert.Id);
        }
    }

    private async Task SendWebhookNotificationAsync(Alert alert, decimal currentPrice)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();

            var payload = new
            {
                alert_id = alert.Id,
                asset_type = alert.AssetType,
                symbol = alert.Symbol,
                condition = alert.Condition,
                target_price = alert.TargetPrice,
                current_price = currentPrice,
                triggered_at = alert.TriggeredAt,
                message = $"Price alert triggered! {alert.Symbol} is {alert.Condition} {alert.TargetPrice:N2} (current: {currentPrice:N2})"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(alert.WebhookUrl, content);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Webhook notification sent to {Url} for alert {AlertId}",
                alert.WebhookUrl, alert.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending webhook notification for alert {AlertId}", alert.Id);
        }
    }

    private Task SendEmailNotificationAsync(Alert alert, decimal currentPrice)
    {
        // Placeholder for email notification
        // In production, integrate with SendGrid, AWS SES, or similar service
        _logger.LogInformation("Email notification would be sent to {Email} for alert {AlertId}",
            alert.Email, alert.Id);
        return Task.CompletedTask;
    }
}
