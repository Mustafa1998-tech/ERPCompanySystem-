using ERPCompanySystem.Models;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendSMSAsync(string to, string message);
        Task SendPushNotificationAsync(string userId, string title, string message);
        Task NotifyLowStockAsync(Product product);
        Task NotifyPaymentDueAsync(Supplier supplier, decimal amount);
    }
}
