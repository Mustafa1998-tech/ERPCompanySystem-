using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using ERPCompanySystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace ERPCompanySystem.Services;

public class AuthenticationService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthenticationService> _logger;
    private static readonly string[] _allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
    private static readonly string[] _disallowedPasswords = new[] { "password", "123456", "qwerty", "admin" };
    private static readonly int _minPasswordLength = 8;
    private static readonly int _maxPasswordLength = 128;
    private static readonly int _minSpecialChars = 1;
    private static readonly int _minNumbers = 1;
    private static readonly int _minUpperCase = 1;
    private static readonly int _minLowerCase = 1;

    public AuthenticationService(IOptions<JwtSettings> jwtSettings, ILogger<AuthenticationService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        
        if (!_jwtSettings.IsValid())
        {
            throw new InvalidOperationException("Invalid JWT settings configuration");
        }
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var random = new Random();
        var bytes = new byte[32];
        random.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public bool ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        if (password.Length < _minPasswordLength || password.Length > _maxPasswordLength)
            return false;

        if (_disallowedPasswords.Any(p => password.Contains(p, StringComparison.OrdinalIgnoreCase)))
            return false;

        var specialChars = password.Count(c => !char.IsLetterOrDigit(c));
        var numbers = password.Count(char.IsDigit);
        var upperCase = password.Count(char.IsUpper);
        var lowerCase = password.Count(char.IsLower);

        return specialChars >= _minSpecialChars &&
               numbers >= _minNumbers &&
               upperCase >= _minUpperCase &&
               lowerCase >= _minLowerCase;
    }
}
