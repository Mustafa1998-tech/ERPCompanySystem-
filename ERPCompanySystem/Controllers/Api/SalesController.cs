using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class SalesController : BaseController
    {
        public SalesController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> GetSales()
        {
            var sales = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .ToListAsync();
            return Response(sales);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.SaleId == id);
            
            if (sale == null) return Error("Sale not found", HttpStatusCode.NotFound);
            return Response(sale);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Sales")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreateSale([FromBody] Sale sale)
        {
            if (!ModelState.IsValid) return Error("Invalid sale data");

            sale.CreatedAt = DateTime.UtcNow;
            
            _context.Sales.Add(sale);
            await SaveChanges();

            return Response(sale, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] Sale sale)
        {
            if (id != sale.SaleId) return Error("ID mismatch");
            if (!ModelState.IsValid) return Error("Invalid sale data");

            var existingSale = await GetEntityById<Sale>(id);
            if (existingSale == null) return Error("Sale not found", HttpStatusCode.NotFound);

            existingSale.ClientId = sale.ClientId;
            existingSale.TotalAmount = sale.TotalAmount;
            existingSale.Status = sale.Status;
            existingSale.SaleDetails = sale.SaleDetails;

            await SaveChanges();
            return Response(existingSale);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await GetEntityById<Sale>(id);
            if (sale == null) return Error("Sale not found", HttpStatusCode.NotFound);

            await DeleteEntity(sale);
            return Response(null, HttpStatusCode.NoContent);
        }
    }
}
