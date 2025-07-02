using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Collections.Generic;

namespace ERPCompanySystem.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly HashSet<string> _sensitiveHeaders = new() { "Authorization", "Cookie", "Set-Cookie" };
    private readonly HashSet<string> _sensitivePaths = new() { 
        "/auth/login", 
        "/auth/register", 
        "/users/*", 
        "/twofactor/*" 
    };

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Log request start
            var startTime = DateTime.UtcNow;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var requestPath = context.Request.Path.ToString();

            // Log request details
            _logger.LogInformation(
                "Request started: {Method} {Path} {Ip} {UserAgent}",
                context.Request.Method,
                requestPath,
                ipAddress,
                userAgent
            );

            // Log request headers (mask sensitive ones)
            var headers = new Dictionary<string, string>();
            foreach (var header in context.Request.Headers)
            {
                headers[header.Key] = _sensitiveHeaders.Contains(header.Key) ? "[REDACTED]" : header.Value.ToString();
            }
            _logger.LogDebug("Request Headers: {@Headers}", headers);

            // Log request body (mask sensitive data)
            var bodyStream = new MemoryStream();
            await context.Request.Body.CopyToAsync(bodyStream);
            bodyStream.Position = 0;
            var body = await new StreamReader(bodyStream).ReadToEndAsync();

            // Mask sensitive data in body
            if (_sensitivePaths.Any(p => requestPath.StartsWith(p)))
            {
                body = MaskSensitiveData(body);
            }

            _logger.LogDebug("Request Body: {Body}", body);

            // Reset stream position
            bodyStream.Position = 0;
            context.Request.Body = bodyStream;

            // Continue processing
            await _next(context);

            // Log response details
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            _logger.LogInformation(
                "Response: {StatusCode} {ContentType} {Duration}ms",
                context.Response.StatusCode,
                context.Response.ContentType,
                duration.TotalMilliseconds
            );

            // Log response headers (mask sensitive ones)
            var responseHeaders = new Dictionary<string, string>();
            foreach (var header in context.Response.Headers)
            {
                responseHeaders[header.Key] = _sensitiveHeaders.Contains(header.Key) ? "[REDACTED]" : header.Value.ToString();
            }
            _logger.LogDebug("Response Headers: {@Headers}", responseHeaders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            throw;
        }
    }

    private string MaskSensitiveData(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            
            if (root.TryGetProperty("password", out var password))
            {
                body = body.Replace(password.GetString(), "[REDACTED]");
            }

            if (root.TryGetProperty("token", out var token))
            {
                body = body.Replace(token.GetString(), "[REDACTED]");
            }

            if (root.TryGetProperty("secret", out var secret))
            {
                body = body.Replace(secret.GetString(), "[REDACTED]");
            }
        }
        catch
        {
            // Ignore JSON parsing errors
        }

        return body;
    }
}
