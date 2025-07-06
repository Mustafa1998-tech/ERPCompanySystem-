using ERPCompanySystem.Models;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public interface IBackupService
    {
        Task<Backup> CreateBackupAsync(BackupType type, string description = "");
        Task<bool> RestoreBackupAsync(int backupId, string description = "");
        Task<IEnumerable<Backup>> GetBackupsAsync();
        Task<Backup> GetBackupByIdAsync(int backupId);
        Task<bool> DeleteBackupAsync(int backupId);
        Task<bool> ValidateBackupAsync(int backupId);
        Task<bool> TestRestoreAsync(int backupId);
    }
}
