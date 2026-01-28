using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using PriceTracker.Api.Data;
using PriceTracker.Api.Endpoints;
using PriceTracker.Api.Jobs;
using PriceTracker.Api.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Redis caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "PriceTracker:";
});

// Add Hangfire
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UsePostgreSqlStorage(c =>
              c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection")));
});

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = builder.Configuration.GetValue<int>("Hangfire:WorkerCount");
    options.SchedulePollingInterval = TimeSpan.FromSeconds(
        builder.Configuration.GetValue<int>("Hangfire:PollingInterval"));
});

// Add HTTP clients with Polly retry policies
builder.Services.AddHttpClient("CoinGecko", client =>
{
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("AlphaVantage", client =>
{
    client.BaseAddress = new Uri("https://www.alphavantage.co/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("ExchangeRate", client =>
{
    client.BaseAddress = new Uri("https://api.exchangerate-api.com/v4/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register services
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IForexService, ForexService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register background jobs
builder.Services.AddScoped<CryptoPriceUpdateJob>();
builder.Services.AddScoped<StockPriceUpdateJob>();
builder.Services.AddScoped<ForexRateUpdateJob>();
builder.Services.AddScoped<AlertCheckJob>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "redis")
    .AddHangfire(options => options.MinimumAvailableServers = 1, name: "hangfire");

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapOpenApi();

app.UseSerilogRequestLogging();



app.UseCors();

// Prometheus metrics endpoint
app.UseMetricServer();
app.UseHttpMetrics();

// Health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Price Tracker Jobs",
    StatsPollingInterval = 2000
});

// Map endpoints
app.MapCryptoEndpoints();
app.MapStockEndpoints();
app.MapForexEndpoints();
app.MapAlertEndpoints();
app.MapStatusEndpoints();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    // Schedule recurring jobs
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<CryptoPriceUpdateJob>(
        "update-crypto-prices",
        job => job.ExecuteAsync(CancellationToken.None),
        builder.Configuration["Jobs:CryptoUpdateCron"]!);

    recurringJobManager.AddOrUpdate<StockPriceUpdateJob>(
        "update-stock-prices",
        job => job.ExecuteAsync(CancellationToken.None),
        builder.Configuration["Jobs:StockUpdateCron"]!);

    recurringJobManager.AddOrUpdate<ForexRateUpdateJob>(
        "update-forex-rates",
        job => job.ExecuteAsync(CancellationToken.None),
        builder.Configuration["Jobs:ForexUpdateCron"]!);

    recurringJobManager.AddOrUpdate<AlertCheckJob>(
        "check-alerts",
        job => job.ExecuteAsync(CancellationToken.None),
        builder.Configuration["Jobs:AlertCheckCron"]!);

    Log.Information("Recurring jobs scheduled successfully");
}

Log.Information("Price Tracker API is starting...");
Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();
