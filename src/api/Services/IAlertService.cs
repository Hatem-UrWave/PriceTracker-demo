using PriceTracker.Api.Models;

namespace PriceTracker.Api.Services;

public interface IAlertService
{
    Task<IEnumerable<Alert>> GetAllAsync();
    Task<Alert?> GetByIdAsync(int id);
    Task<Alert> CreateAsync(CreateAlertRequest request);
    Task DeleteAsync(int id);
    Task CheckAlertsAsync();
}
