using PriceTracker.Api.Services;

namespace PriceTracker.Api.Endpoints;

public static class ForexEndpoints
{
    public static void MapForexEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/forex").WithTags("Forex");

        group.MapGet("/", async (IForexService service) =>
        {
            var rates = await service.GetAllAsync();
            return Results.Ok(rates);
        })
        .WithName("GetAllForexRates")
        .WithOpenApi();

        group.MapGet("/{base}/{target}", async (string @base, string target, IForexService service) =>
        {
            var rate = await service.GetRateAsync(@base, target);
            return rate is not null ? Results.Ok(rate) : Results.NotFound();
        })
        .WithName("GetForexRate")
        .WithOpenApi();
    }
}
