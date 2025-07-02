using Microsoft.AspNetCore.Mvc;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text.Json;

namespace ERPCompanySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtSettings _jwtSettings;

        public AuthController(AppDbContext context, IConfiguration config, ILogger<AuthController> logger, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            try
            {
                if (loginUser == null)
                {
                    return BadRequest(new { message = "البيانات غير صالحة" });
                }

                if (string.IsNullOrWhiteSpace(loginUser.Username) || string.IsNullOrWhiteSpace(loginUser.Password))
                {
                    return BadRequest(new { message = "يرجى إدخال اسم المستخدم وكلمة المرور." });
                }

                // التحقق من IP blocking
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var isBlocked = await _context.IpBlocks.AnyAsync(b => 
                    b.IpAddress == ip && b.BlockedUntil > DateTime.UtcNow);

                if (isBlocked)
                {
                    return StatusCode(429, new { message = "IP محظور مؤقتاً" });
                }

                // التحقق من محاولات تسجيل الدخول الفاشلة
                var failedAttempts = await _context.LoginAttempts
                    .Where(la => la.IpAddress == ip && 
                               la.Username == loginUser.Username && 
                               la.AttemptTime > DateTime.UtcNow.AddMinutes(-15))
                    .CountAsync();

                if (failedAttempts >= 5)
                {
                    var blockUntil = DateTime.UtcNow.AddMinutes(15);
                    await _context.IpBlocks.AddAsync(new IpBlock
                    {
                        IpAddress = ip,
                        BlockedUntil = blockUntil
                    });
                    await _context.SaveChangesAsync();
                    return StatusCode(429, new { message = "كثرة محاولات تسجيل الدخول الفاشلة" });
                }

                // البحث عن المستخدم
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginUser.Username);

                if (existingUser == null)
                {
                    await _context.LoginAttempts.AddAsync(new LoginAttempt
                    {
                        IpAddress = ip,
                        Username = loginUser.Username,
                        AttemptTime = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                    return Unauthorized(new { message = "خطأ في اسم المستخدم أو كلمة المرور." });
                }

                // التحقق من كلمة المرور
                if (!BCrypt.Net.BCrypt.Verify(loginUser.Password, existingUser.PasswordHash))
                {
                    await _context.LoginAttempts.AddAsync(new LoginAttempt
                    {
                        IpAddress = ip,
                        Username = loginUser.Username,
                        AttemptTime = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                    return Unauthorized(new { message = "خطأ في اسم المستخدم أو كلمة المرور." });
                }

                // توليد التوكنات
                var (jwtToken, refreshToken) = GenerateTokens(existingUser);

                // تحديث refresh token في قاعدة البيانات
                existingUser.RefreshToken = refreshToken;
                existingUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    token = jwtToken,
                    refreshToken = refreshToken,
                    tokenExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                    refreshTokenExpires = existingUser.RefreshTokenExpiryTime
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تسجيل الدخول");
                return StatusCode(500, new { message = "خطأ في النظام" });
            }
        }
        }

        private (string, string) GenerateTokens(User user)
        {
            try
            {
                // توليد JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("jti", Guid.NewGuid().ToString()),
                        new Claim("iat", DateTime.UtcNow.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                // توليد refresh token
                using var rng = RandomNumberGenerator.Create();
                var randomBytes = new byte[32];
                rng.GetBytes(randomBytes);
                var refreshToken = Convert.ToBase64String(randomBytes);

                return (jwtToken, refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء توليد التوكنات");
                throw;
            }
        }
    }
}
