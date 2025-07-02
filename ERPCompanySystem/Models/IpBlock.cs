using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCompanySystem.Models;

public class IpBlock
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    public DateTime BlockedUntil { get; set; }
}
