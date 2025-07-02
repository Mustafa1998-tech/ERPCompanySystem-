using AspNetCoreRateLimit;

namespace ERPCompanySystem.Configuration
{
    public class RateLimitConfiguration
    {
        public static void ConfigureRateLimitingOptions(IpRateLimitOptions options)
        {
            options.EnableEndpointRateLimiting = true;
            options.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "api/auth/login",
                    Period = "5m",
                    Limit = 10,
                    IpPolicy = true
                },
                new RateLimitRule
                {
                    Endpoint = "api/[controller]",
                    Period = "15m",
                    Limit = 100,
                    IpPolicy = true
                }
            };
        }
    }
}
