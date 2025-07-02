using Microsoft.AspNetCore.Mvc;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using System.Security.Claims;
using ERPCompanySystem.Attributes;
using System.Linq;

namespace ERPCompanySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CustomAuthorize(new string[] { "Manager", "Admin" })]  // Only Manager and Admin can access
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reports/DailySales
        [HttpGet("DailySales")]
        public async Task<ActionResult> GetDailySales()
        {
            var today = DateTime.Now.Date;
            var sales = await _context.Sales
                .Where(s => s.SaleDate.Date == today)
                .Include(s => s.Product)
                .ToListAsync();

            var report = new
            {
                Date = today,
                TotalSales = sales.Sum(s => s.Quantity * s.UnitPrice),
                TotalDiscount = sales.Sum(s => s.Discount),
                TotalItems = sales.Sum(s => s.Quantity),
                Products = sales.GroupBy(s => s.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        ProductName = g.First().Product.Name,
                        TotalQuantity = g.Sum(s => s.Quantity),
                        TotalRevenue = g.Sum(s => s.Quantity * s.UnitPrice)
                    })
            };

            return Ok(report);
        }

        // GET: api/Reports/MonthlySales
        [HttpGet("MonthlySales")]
        public async Task<ActionResult> GetMonthlySales()
        {
            var startDate = DateTime.Now.Date.AddDays(-30);
            var sales = await _context.Sales
                .Where(s => s.SaleDate >= startDate)
                .Include(s => s.Product)
                .ToListAsync();

            var report = new
            {
                StartDate = startDate,
                EndDate = DateTime.Now.Date,
                TotalSales = sales.Sum(s => s.Quantity * s.UnitPrice),
                TotalDiscount = sales.Sum(s => s.Discount),
                DailySales = sales.GroupBy(s => s.SaleDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalSales = g.Sum(s => s.Quantity * s.UnitPrice),
                        TotalItems = g.Sum(s => s.Quantity)
                    })
                    .OrderBy(d => d.Date)
            };

            return Ok(report);
        }

        // GET: api/Reports/InventoryStatus
        [HttpGet("InventoryStatus")]
        public async Task<ActionResult> GetInventoryStatus()
        {
            var products = await _context.Products.ToListAsync();
            var movements = await _context.StockMovements
                .Include(m => m.Product)
                .OrderByDescending(m => m.MovementDate)
                .Take(100)
                .ToListAsync();

            var report = new
            {
                Products = products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.StockQuantity,
                    p.Unit,
                    LastMovement = movements.FirstOrDefault(m => m.ProductId == p.Id)
                }),
                RecentMovements = movements
            };

            return Ok(report);
        }
    }
}
