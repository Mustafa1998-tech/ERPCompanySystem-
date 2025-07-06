using ERPCompanySystem.Models;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public interface IAuditService
    {
        Task LogAsync(string userId, string username, string action, string tableName, 
            string? recordId = null, string? oldValues = null, string? newValues = null,
            string? changedColumns = null, string? changeReason = null);

        Task<AuditLog> GetLogAsync(int id);
        Task<IEnumerable<AuditLog>> GetLogsAsync(string? userId = null, string? action = null,
            string? tableName = null, DateTime? startDate = null, DateTime? endDate = null);
    }
}
