using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCompanySystem.Models;

public class LoginAttempt
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    public DateTime AttemptTime { get; set; }
}
