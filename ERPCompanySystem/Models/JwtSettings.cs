namespace ERPCompanySystem.Models;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;

    public bool IsValid() =>
        !string.IsNullOrEmpty(Secret) &&
        !string.IsNullOrEmpty(Issuer) &&
        !string.IsNullOrEmpty(Audience);
}
