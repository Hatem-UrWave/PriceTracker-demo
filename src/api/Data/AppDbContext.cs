using Microsoft.EntityFrameworkCore;
using PriceTracker.Api.Models;

namespace PriceTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CryptoPrice> CryptoPrices { get; set; }
    public DbSet<StockPrice> StockPrices { get; set; }
    public DbSet<ForexRate> ForexRates { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CryptoPrice configuration
        modelBuilder.Entity<CryptoPrice>(entity =>
        {
            entity.ToTable("crypto_prices");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Symbol).IsUnique();
            entity.Property(e => e.PriceUsd).HasPrecision(18, 8);
            entity.Property(e => e.PriceEur).HasPrecision(18, 8);
            entity.Property(e => e.MarketCapUsd).HasPrecision(20, 2);
            entity.Property(e => e.Volume24hUsd).HasPrecision(20, 2);
            entity.Property(e => e.ChangePercent24h).HasPrecision(10, 4);
        });

        // StockPrice configuration
        modelBuilder.Entity<StockPrice>(entity =>
        {
            entity.ToTable("stock_prices");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Symbol).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 4);
            entity.Property(e => e.DayHigh).HasPrecision(18, 4);
            entity.Property(e => e.DayLow).HasPrecision(18, 4);
            entity.Property(e => e.Open).HasPrecision(18, 4);
            entity.Property(e => e.PreviousClose).HasPrecision(18, 4);
            entity.Property(e => e.ChangePercent).HasPrecision(10, 4);
        });

        // ForexRate configuration
        modelBuilder.Entity<ForexRate>(entity =>
        {
            entity.ToTable("forex_rates");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.BaseCurrency, e.TargetCurrency }).IsUnique();
            entity.Property(e => e.Rate).HasPrecision(18, 8);
        });

        // Alert configuration
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.ToTable("alerts");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.AssetType, e.Symbol, e.IsActive });
            entity.Property(e => e.TargetPrice).HasPrecision(18, 8);
        });
    }
}
