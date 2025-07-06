using ERPCompanySystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportSalesToExcelAsync(IEnumerable<Sale> sales);
        Task<byte[]> ExportProductsToExcelAsync(IEnumerable<Product> products);
        Task<byte[]> ExportInventoryToExcelAsync(IEnumerable<Product> inventory);
        Task<byte[]> ExportFinancialReportToExcelAsync(
            DateTime startDate,
            DateTime endDate,
            decimal totalSales,
            decimal totalPurchases,
            decimal netProfit);
    }
}
