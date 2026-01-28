using PriceTracker.Api.Models;
using PriceTracker.Api.Services;

namespace PriceTracker.Api.Endpoints;

public static class AlertEndpoints
{
    public static void MapAlertEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/alerts").WithTags("Alerts");

        group.MapGet("/", async (IAlertService service) =>
        {
            var alerts = await service.GetAllAsync();
            return Results.Ok(alerts);
        })
        .WithName("GetAllAlerts")
        .WithOpenApi();

        group.MapGet("/{id:int}", async (int id, IAlertService service) =>
        {
            var alert = await service.GetByIdAsync(id);
            return alert is not null ? Results.Ok(alert) : Results.NotFound();
        })
        .WithName("GetAlertById")
        .WithOpenApi();

        group.MapPost("/", async (CreateAlertRequest request, IAlertService service) =>
        {
            var alert = await service.CreateAsync(request);
            return Results.Created($"/api/alerts/{alert.Id}", alert);
        })
        .WithName("CreateAlert")
        .WithOpenApi();

        group.MapDelete("/{id:int}", async (int id, IAlertService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteAlert")
        .WithOpenApi();
    }
}
