using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCompanySystem.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Action { get; set; } = null!;

        [Required]
        public string TableName { get; set; } = null!;

        public string? RecordId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? ChangedColumns { get; set; }

        public string? ChangeReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? IPAddress { get; set; }

        public string? ApplicationName { get; set; }
    }
}
