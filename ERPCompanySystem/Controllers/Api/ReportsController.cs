using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;
using System.Linq.Dynamic.Core;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class ReportsController : BaseController
    {
        public ReportsController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet("sales")]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> GetSalesReport(
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

            var sales = await query
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return Response(new
            {
                totalSales = sales.Count,
                totalAmount = sales.Sum(s => s.TotalAmount),
                sales
            });
        }

        [HttpGet("inventory")]
        [Authorize(Roles = "Admin,Manager,Inventory")]
        public async Task<IActionResult> GetInventoryReport(
            string? warehouseId = null,
            string? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.Warehouse);

            if (!string.IsNullOrEmpty(warehouseId))
                query = query.Where(p => p.WarehouseId.ToString() == warehouseId);

            if (!string.IsNullOrEmpty(categoryId))
                query = query.Where(p => p.CategoryId.ToString() == categoryId);

            var inventory = await query
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    p.CreatedAt,
                    Warehouse = new
                    {
                        p.Warehouse.WarehouseId,
                        p.Warehouse.Name,
                        p.Warehouse.Location
                    }
                })
                .ToListAsync();

            return Response(new
            {
                totalProducts = inventory.Count,
                totalValue = inventory.Sum(p => p.Price * p.StockQuantity),
                inventory
            });
        }

        [HttpGet("financial")]
        [Authorize(Roles = "Admin,Manager,Accountant")]
        public async Task<IActionResult> GetFinancialReport(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var salesQuery = _context.Sales
                .Include(s => s.Payment)
                .Where(s => !startDate.HasValue || s.CreatedAt >= startDate.Value)
                .Where(s => !endDate.HasValue || s.CreatedAt <= endDate.Value);

            var purchasesQuery = _context.Purchases
                .Include(p => p.Supplier)
                .Where(p => !startDate.HasValue || p.CreatedAt >= startDate.Value)
                .Where(p => !endDate.HasValue || p.CreatedAt <= endDate.Value);

            var sales = await salesQuery.ToListAsync();
            var purchases = await purchasesQuery.ToListAsync();

            return Response(new
            {
                sales = new
                {
                    total = sales.Count,
                    amount = sales.Sum(s => s.TotalAmount),
                    payments = sales.Count(s => s.Payment != null)
                },
                purchases = new
                {
                    total = purchases.Count,
                    amount = purchases.Sum(p => p.TotalAmount)
                },
                netProfit = sales.Sum(s => s.TotalAmount) - purchases.Sum(p => p.TotalAmount)
            });
        }
    }
}
