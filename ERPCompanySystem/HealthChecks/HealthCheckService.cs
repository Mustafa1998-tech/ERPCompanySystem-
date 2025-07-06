using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace ERPCompanySystem.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _context;

        public DatabaseHealthCheck(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.CanConnectAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection is healthy");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(
                    context.Registration.FailureStatus,
                    "Database connection failed",
                    exception: ex,
                    duration: context.Stopwatch.Elapsed);
            }
        }
    }

    public class CacheHealthCheck : IHealthCheck
    {
        private readonly IDistributedCache _cache;

        public CacheHealthCheck(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var testKey = "health_check_test";
                var testValue = "test";
                await _cache.SetStringAsync(testKey, testValue, cancellationToken);
                var result = await _cache.GetStringAsync(testKey, cancellationToken);

                if (result == testValue)
                {
                    return HealthCheckResult.Healthy("Cache is healthy");
                }

                return HealthCheckResult.Unhealthy("Cache test failed");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(
                    context.Registration.FailureStatus,
                    "Cache connection failed",
                    exception: ex,
                    duration: context.Stopwatch.Elapsed);
            }
        }
    }
}
