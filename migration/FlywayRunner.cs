using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.migration;

/// <summary>
/// معادل FlywayRunner (ApplicationReadyEvent): پس از آماده شدن برنامه، مایگریشن‌ها را با
/// حداکثر ۱۰ بار تلاش مجدد اجرا می‌کند. به‌جای Flyway از EF Core migrations استفاده شده است.
/// </summary>
public class FlywayRunner : IHostedService
{
    private const int MaxRetries = 10;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(5000);

    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<FlywayRunner> _logger;

    public FlywayRunner(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<FlywayRunner> logger)
    {
        _services = services;
        _lifetime = lifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(() => RunAsync(_lifetime.ApplicationStopping));
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task RunAsync(CancellationToken ct)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation("🔄 Trying to run migration (attempt {Attempt}/{Max})", attempt, MaxRetries);

                await using var scope = _services.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<SemDbContext>();

                // await scope.ServiceProvider.GetRequiredService<SupremeBackfillService>().RunBackfill();

                await context.Database.MigrateAsync(ct);

                _logger.LogInformation("✅ Migration completed successfully!");
                return;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ Migration failed on attempt {Attempt}: {Message}", attempt, ex.Message);

                if (attempt == MaxRetries)
                {
                    _logger.LogCritical(ex, "❌ Maximum retry attempts reached. Migration failed permanently.");
                    throw new InvalidOperationException("Migration failed after multiple attempts", ex);
                }

                try
                {
                    await Task.Delay(RetryDelay, ct);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }
}
