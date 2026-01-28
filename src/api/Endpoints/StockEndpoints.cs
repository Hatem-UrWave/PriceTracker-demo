using PriceTracker.Api.Services;

namespace PriceTracker.Api.Endpoints;

public static class StockEndpoints
{
    public static void MapStockEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stocks").WithTags("Stocks");

        group.MapGet("/", async (IStockService service) =>
        {
            var prices = await service.GetAllAsync();
            return Results.Ok(prices);
        })
        .WithName("GetAllStocks")
        .WithOpenApi();

        group.MapGet("/{symbol}", async (string symbol, IStockService service) =>
        {
            var price = await service.GetBySymbolAsync(symbol);
            return price is not null ? Results.Ok(price) : Results.NotFound();
        })
        .WithName("GetStockBySymbol")
        .WithOpenApi();
    }
}
