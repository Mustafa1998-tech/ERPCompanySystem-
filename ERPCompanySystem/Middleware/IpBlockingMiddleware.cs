using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;

namespace ERPCompanySystem.Middleware;

public class IpBlockingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBlockingMiddleware> _logger;
    private readonly AppDbContext _context;
    private readonly HashSet<string> _blockedIps = new();
    private readonly HashSet<string> _whitelistedIps = new() { "127.0.0.1", "::1" };

    public IpBlockingMiddleware(RequestDelegate next, ILogger<IpBlockingMiddleware> logger, AppDbContext context)
    {
        _next = next;
        _logger = logger;
        _context = context;

        // Load blocked IPs from database
        LoadBlockedIps();
    }

    private void LoadBlockedIps()
    {
        var blockedIps = _context.IpBlocks
            .Where(ip => ip.ExpiryTime > DateTime.UtcNow)
            .Select(ip => ip.IpAddress)
            .ToList();

        _blockedIps.Clear();
        foreach (var ip in blockedIps)
        {
            _blockedIps.Add(ip);
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        
        if (clientIp == null)
        {
            _logger.LogWarning("Could not determine client IP address");
            await _next(context);
            return;
        }

        if (_whitelistedIps.Contains(clientIp))
        {
            await _next(context);
            return;
        }

        if (_blockedIps.Contains(clientIp))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = true,
                message = "Your IP address has been blocked.",
                timestamp = DateTime.UtcNow
            });
            return;
        }

        await _next(context);
    }

    public void BlockIp(string ipAddress, TimeSpan duration)
    {
        if (_whitelistedIps.Contains(ipAddress))
        {
            _logger.LogWarning("Attempt to block whitelisted IP: {IpAddress}", ipAddress);
            return;
        }

        var ipBlock = new IpBlock
        {
            IpAddress = ipAddress,
            ExpiryTime = DateTime.UtcNow.Add(duration)
        };

        _context.IpBlocks.Add(ipBlock);
        _context.SaveChanges();
        _blockedIps.Add(ipAddress);

        _logger.LogInformation("IP address {IpAddress} has been blocked for {Duration} minutes", ipAddress, duration.TotalMinutes);
    }

    public void UnblockIp(string ipAddress)
    {
        var ipBlock = _context.IpBlocks.FirstOrDefault(ip => ip.IpAddress == ipAddress);
        if (ipBlock != null)
        {
            _context.IpBlocks.Remove(ipBlock);
            _context.SaveChanges();
            _blockedIps.Remove(ipAddress);
            _logger.LogInformation("IP address {IpAddress} has been unblocked", ipAddress);
        }
    }

    public void CleanExpiredBlocks()
    {
        var expiredBlocks = _context.IpBlocks
            .Where(ip => ip.ExpiryTime <= DateTime.UtcNow)
            .ToList();

        foreach (var ip in expiredBlocks)
        {
            _context.IpBlocks.Remove(ip);
            _blockedIps.Remove(ip.IpAddress);
        }

        if (expiredBlocks.Any())
        {
            _context.SaveChanges();
            _logger.LogInformation("Cleaned {Count} expired IP blocks", expiredBlocks.Count);
        }
    }
}
