using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCompanySystem.Models
{
    [Table("Backups")]
    public class Backup
    {
        [Key]
        public int BackupId { get; set; }

        [Required]
        public string FileName { get; set; } = null!;

        [Required]
        public string FilePath { get; set; } = null!;

        public string? Description { get; set; }

        public long FileSize { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RestoredAt { get; set; }

        public string? RestoredBy { get; set; }

        public bool IsSuccessful { get; set; }

        public string? ErrorDetails { get; set; }

        public BackupType Type { get; set; }

        public enum BackupType
        {
            Full,
            Differential,
            TransactionLog
        }
    }
}
