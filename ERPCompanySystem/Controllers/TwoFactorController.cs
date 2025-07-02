using Microsoft.AspNetCore.Mvc;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Services;
using ERPCompanySystem.Attributes;
using System.Security.Claims;

namespace ERPCompanySystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TwoFactorController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthenticationService _authService;
    private readonly ILogger<TwoFactorController> _logger;

    public TwoFactorController(
        AppDbContext context,
        AuthenticationService authService,
        ILogger<TwoFactorController> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("setup")]
    [CustomAuthorize(new string[] { "User" })]
    public IActionResult Setup2FA()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = _context.Users.Find(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var (secret, qrCode) = _authService.Generate2FASecret();
        user.TwoFactorSecret = secret;
        _context.SaveChanges();

        return Ok(new
        {
            secret,
            qrCode,
            instructions = "Scan this QR code with your authenticator app"
        });
    }

    [HttpPost("verify")]
    [CustomAuthorize(new string[] { "User" })]
    public IActionResult Verify2FA([FromBody] string token)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = _context.Users.Find(userId);

        if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return NotFound("2FA not set up");
        }

        if (!_authService.Verify2FAToken(user.TwoFactorSecret, token))
        {
            return BadRequest("Invalid 2FA token");
        }

        return Ok("2FA verified successfully");
    }

    [HttpPost("enable")]
    [CustomAuthorize(new string[] { "User" })]
    public IActionResult Enable2FA([FromBody] string token)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = _context.Users.Find(userId);

        if (user == null || string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return NotFound("2FA not set up");
        }

        if (!_authService.Verify2FAToken(user.TwoFactorSecret, token))
        {
            return BadRequest("Invalid 2FA token");
        }

        user.Is2FAEnabled = true;
        _context.SaveChanges();

        return Ok("2FA enabled successfully");
    }

    [HttpPost("disable")]
    [CustomAuthorize(new string[] { "User" })]
    public IActionResult Disable2FA()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = _context.Users.Find(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        user.Is2FAEnabled = false;
        user.TwoFactorSecret = null;
        _context.SaveChanges();

        return Ok("2FA disabled successfully");
    }
}
