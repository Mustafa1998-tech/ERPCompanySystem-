using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace ERPCompanySystem.Models;

public class User : IIdentity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Index(IsUnique = true)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Index(IsUnique = true)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string PasswordHash { get; set; } = string.Empty;

    [NotMapped]
    public string? Password { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "User";

    public string? ProfilePictureUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLogin { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    [NotMapped]
    public string? Token { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>(); 

    [StringLength(32)]
    public string? TwoFactorSecret { get; set; }

    public bool Is2FAEnabled { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? LastLogin { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    [NotMapped]
    public string? Token { get; set; }

    [NotMapped]
    public string? CurrentRefreshToken { get; set; }

    [NotMapped]
    public DateTime? TokenExpires { get; set; }

    [NotMapped]
    public string? LastIP { get; set; }

    [NotMapped]
    public string? LastUserAgent { get; set; }

    [NotMapped]
    public bool IsLockedOut { get; set; }

    [NotMapped]
    public DateTime? LockoutEnd { get; set; }

    [NotMapped]
    public int FailedLoginAttempts { get; set; }

    [NotMapped]
    public DateTime? LastFailedLogin { get; set; }

    public bool IsActive { get; set; } = true;
}
