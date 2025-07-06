using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Services;
using ERPCompanySystem.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly AppDbContext _context;

        public ExportController(IExportService exportService, AppDbContext context)
        {
            _exportService = exportService;
            _context = context;
        }

        [HttpGet("sales")]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> ExportSales(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? clientId = null)
        {
            var query = _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product);

            if (startDate.HasValue)
                query = query.Where(s => s.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.CreatedAt <= endDate.Value);

            if (!string.IsNullOrEmpty(clientId))
                query = query.Where(s => s.ClientId.ToString() == clientId);

            var sales = await query.ToListAsync();
            var fileName = $"Sales_Report_{DateTime.Now:yyyyMMdd}.xlsx";
            var fileBytes = await _exportService.ExportSalesToExcelAsync(sales);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("inventory")]
        [Authorize(Roles = "Admin,Manager,Inventory")]
        public async Task<IActionResult> ExportInventory(
            string? warehouseId = null,
            string? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.Warehouse);

            if (!string.IsNullOrEmpty(warehouseId))
                query = query.Where(p => p.WarehouseId.ToString() == warehouseId);

            if (!string.IsNullOrEmpty(categoryId))
                query = query.Where(p => p.CategoryId.ToString() == categoryId);

            var inventory = await query.ToListAsync();
            var fileName = $"Inventory_Report_{DateTime.Now:yyyyMMdd}.xlsx";
            var fileBytes = await _exportService.ExportInventoryToExcelAsync(inventory);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("financial")]
        [Authorize(Roles = "Admin,Manager,Accountant")]
        public async Task<IActionResult> ExportFinancialReport(
            DateTime startDate,
            DateTime endDate)
        {
            var sales = await _context.Sales
                .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
                .ToListAsync();

            var purchases = await _context.Purchases
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .ToListAsync();

            var totalSales = sales.Sum(s => s.TotalAmount);
            var totalPurchases = purchases.Sum(p => p.TotalAmount);
            var netProfit = totalSales - totalPurchases;

            var fileName = $"Financial_Report_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.xlsx";
            var fileBytes = await _exportService.ExportFinancialReportToExcelAsync(
                startDate,
                endDate,
                totalSales,
                totalPurchases,
                netProfit);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
