using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ERPCompanySystem.BackgroundServices
{
    public class StockMonitorService : BackgroundService
    {
        private readonly ILogger<StockMonitorService> _logger;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public StockMonitorService(
            ILogger<StockMonitorService> logger,
            AppDbContext context,
            INotificationService notificationService)
        {
            _logger = logger;
            _context = context;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckLowStockAsync();
                    await CheckExpiredProductsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in StockMonitorService");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckLowStockAsync()
        {
            var lowStockThreshold = 10; // من الممكن جعل هذا قابل للتكوين
            var lowStockProducts = await _context.Products
                .Where(p => p.StockQuantity <= lowStockThreshold)
                .ToListAsync();

            foreach (var product in lowStockProducts)
            {
                await _notificationService.NotifyLowStockAsync(product);
            }
        }

        private async Task CheckExpiredProductsAsync()
        {
            var expiredProducts = await _context.Products
                .Where(p => p.ExpiryDate.HasValue && 
                           p.ExpiryDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var product in expiredProducts)
            {
                // تنفيذ المنطق المطلوب للمنتجات منتهية الصلاحية
                // مثل إرسال إشعارات أو تحديث حالة المنتج
            }
        }
    }
}
