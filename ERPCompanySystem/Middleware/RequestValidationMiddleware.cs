using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Collections.Generic;

namespace ERPCompanySystem.Middleware;

public class RequestValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestValidationMiddleware> _logger;
    private readonly Dictionary<string, int> _requestCounts = new();
    private readonly Dictionary<string, DateTime> _lastRequestTimes = new();
    private const int MAX_REQUESTS_PER_MINUTE = 100;

    public RequestValidationMiddleware(RequestDelegate next, ILogger<RequestValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var requestKey = $"{ipAddress}:{userAgent}:{context.Request.Path}";

            // Rate limiting
            if (await IsRateLimited(requestKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            // Validate content type
            if (context.Request.ContentType == null || !context.Request.ContentType.Contains("application/json"))
            {
                _logger.LogWarning("Invalid content type: {ContentType}", context.Request.ContentType);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid content type. Expected application/json.");
                return;
            }

            // Read request body
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            
            // Validate request size
            if (body.Length > 1024 * 1024) // 1MB limit
            {
                _logger.LogWarning("Request body too large: {Size} bytes", body.Length);
                context.Response.StatusCode = 413;
                await context.Response.WriteAsync("Request body too large.");
                return;
            }

            // Validate JSON structure
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                
                // Validate common fields
                if (context.Request.Path.Contains("/auth/"))
                {
                    if (!root.TryGetProperty("username", out _))
                    {
                        _logger.LogWarning("Missing required field: username");
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Missing required field: username");
                        return;
                    }

                    if (!root.TryGetProperty("password", out _))
                    {
                        _logger.LogWarning("Missing required field: password");
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Missing required field: password");
                        return;
                    }
                }
            }
            catch (JsonException)
            {
                _logger.LogWarning("Invalid JSON format");
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid JSON format");
                return;
            }

            // Reset stream position
            context.Request.Body.Position = 0;

            // Continue to next middleware
            await _next(context);

            // Track successful request
            await TrackRequest(requestKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in request validation middleware");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    }

    private async Task<bool> IsRateLimited(string requestKey)
    {
        var currentTime = DateTime.UtcNow;
        var lastTime = _lastRequestTimes.GetValueOrDefault(requestKey, DateTime.MinValue);
        
        // Reset counter if time window has passed
        if ((currentTime - lastTime).TotalMinutes >= 1)
        {
            _requestCounts[requestKey] = 0;
            _lastRequestTimes[requestKey] = currentTime;
        }

        // Increment request count
        _requestCounts[requestKey] = _requestCounts.GetValueOrDefault(requestKey, 0) + 1;

        return _requestCounts[requestKey] > MAX_REQUESTS_PER_MINUTE;
    }

    private async Task TrackRequest(string requestKey)
    {
        // TODO: Implement request tracking
        // This could be stored in a database or cache
    }
}
