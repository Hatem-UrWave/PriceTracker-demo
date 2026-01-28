using Microsoft.AspNetCore.Mvc;
using PriceTracker.Api.Services;

namespace PriceTracker.Api.Endpoints;

public static class CryptoEndpoints
{
    public static void MapCryptoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/crypto").WithTags("Cryptocurrency");

        group.MapGet("/", async (ICryptoService service) =>
        {
            var prices = await service.GetAllAsync();
            return Results.Ok(prices);
        })
        .WithName("GetAllCryptocurrencies")
        .WithOpenApi();

        group.MapGet("/top/{count:int}", async (int count, ICryptoService service) =>
        {
            var prices = await service.GetTopAsync(count);
            return Results.Ok(prices);
        })
        .WithName("GetTopCryptocurrencies")
        .WithOpenApi();

        group.MapGet("/{symbol}", async (string symbol, ICryptoService service) =>
        {
            var price = await service.GetBySymbolAsync(symbol);
            return price is not null ? Results.Ok(price) : Results.NotFound();
        })
        .WithName("GetCryptocurrencyBySymbol")
        .WithOpenApi();
    }
}
