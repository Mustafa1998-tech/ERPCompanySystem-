using ERPCompanySystem.Models;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public class BackupService : IBackupService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<BackupService> _logger;

        public BackupService(AppDbContext context, IConfiguration config, ILogger<BackupService> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        public async Task<Backup> CreateBackupAsync(BackupType type, string description = "")
        {
            var backupDir = _config["Backup:Directory"] ?? 
                Path.Combine(Directory.GetCurrentDirectory(), "backups");

            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            var connectionString = _context.Database.GetDbConnection().ConnectionString;
            var backupFile = Path.Combine(backupDir, 
                $"erp_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = type switch
            {
                BackupType.Full => $"BACKUP DATABASE [{_config["ConnectionStrings:DefaultConnection"]?.Split(';')[0].Split('=')[1]}] TO DISK = '{backupFile}' WITH FORMAT, MEDIANAME = 'ERPBackups', NAME = 'ERPCompanySystem-Full Backup'",
                BackupType.Differential => $"BACKUP DATABASE [{_config["ConnectionStrings:DefaultConnection"]?.Split(';')[0].Split('=')[1]}] TO DISK = '{backupFile}' WITH DIFFERENTIAL, FORMAT, MEDIANAME = 'ERPBackups', NAME = 'ERPCompanySystem-Differential Backup'",
                BackupType.TransactionLog => $"BACKUP LOG [{_config["ConnectionStrings:DefaultConnection"]?.Split(';')[0].Split('=')[1]}] TO DISK = '{backupFile}' WITH FORMAT, MEDIANAME = 'ERPBackups', NAME = 'ERPCompanySystem-Transaction Log Backup'",
                _ => throw new NotSupportedException($"Backup type {type} is not supported")
            };

            try
            {
                using var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                var backup = new Backup
                {
                    FileName = Path.GetFileName(backupFile),
                    FilePath = backupFile,
                    Description = description,
                    FileSize = new FileInfo(backupFile).Length,
                    Type = type,
                    IsSuccessful = true
                };

                await _context.Backups.AddAsync(backup);
                await _context.SaveChangesAsync();

                return backup;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating backup");
                throw;
            }
        }

        public async Task<bool> RestoreBackupAsync(int backupId, string description = "")
        {
            var backup = await _context.Backups.FindAsync(backupId);
            if (backup == null) return false;

            var connectionString = _context.Database.GetDbConnection().ConnectionString;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = $"RESTORE DATABASE [{_config["ConnectionStrings:DefaultConnection"]?.Split(';')[0].Split('=')[1]}] FROM DISK = '{backup.FilePath}' WITH REPLACE, RECOVERY, STATS = 10";

            try
            {
                using var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                backup.RestoredAt = DateTime.UtcNow;
                backup.RestoredBy = _config["User:Id"];
                backup.IsSuccessful = true;
                backup.ErrorDetails = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                backup.RestoredAt = DateTime.UtcNow;
                backup.RestoredBy = _config["User:Id"];
                backup.IsSuccessful = false;
                backup.ErrorDetails = ex.Message;

                await _context.SaveChangesAsync();
                _logger.LogError(ex, "Error restoring backup");
                return false;
            }
        }

        public async Task<IEnumerable<Backup>> GetBackupsAsync()
        {
            return await _context.Backups
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Backup> GetBackupByIdAsync(int backupId)
        {
            return await _context.Backups.FindAsync(backupId);
        }

        public async Task<bool> DeleteBackupAsync(int backupId)
        {
            var backup = await _context.Backups.FindAsync(backupId);
            if (backup == null) return false;

            try
            {
                File.Delete(backup.FilePath);
                _context.Backups.Remove(backup);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting backup");
                return false;
            }
        }

        public async Task<bool> ValidateBackupAsync(int backupId)
        {
            var backup = await _context.Backups.FindAsync(backupId);
            if (backup == null || !File.Exists(backup.FilePath)) return false;

            var connectionString = _context.Database.GetDbConnection().ConnectionString;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = $"RESTORE VERIFYONLY FROM DISK = '{backup.FilePath}'";

            try
            {
                using var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TestRestoreAsync(int backupId)
        {
            var backup = await _context.Backups.FindAsync(backupId);
            if (backup == null) return false;

            var connectionString = _context.Database.GetDbConnection().ConnectionString;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var tempDbName = $"ERPCompanySystem_Test_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
            var query = $"RESTORE DATABASE {tempDbName} FROM DISK = '{backup.FilePath}' WITH REPLACE, RECOVERY, STATS = 10";

            try
            {
                using var command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                // إزالة قاعدة البيانات التجريبية
                query = $"DROP DATABASE {tempDbName}";
                command = new SqlCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
