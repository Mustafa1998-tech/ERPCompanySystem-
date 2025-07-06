using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class SuppliersController : BaseController
    {
        public SuppliersController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        public async Task<IActionResult> GetSuppliers()
        {
            var suppliers = await _context.Suppliers
                .OrderBy(s => s.Name)
                .ToListAsync();
            return Response(suppliers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Purchases)
                .FirstOrDefaultAsync(s => s.SupplierId == id);
            
            if (supplier == null) return Error("Supplier not found", HttpStatusCode.NotFound);
            return Response(supplier);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid) return Error("Invalid supplier data");

            supplier.CreatedAt = DateTime.UtcNow;
            
            _context.Suppliers.Add(supplier);
            await SaveChanges();

            return Response(supplier, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.SupplierId) return Error("ID mismatch");
            if (!ModelState.IsValid) return Error("Invalid supplier data");

            var existingSupplier = await GetEntityById<Supplier>(id);
            if (existingSupplier == null) return Error("Supplier not found", HttpStatusCode.NotFound);

            existingSupplier.Name = supplier.Name;
            existingSupplier.Email = supplier.Email;
            existingSupplier.Phone = supplier.Phone;
            existingSupplier.Address = supplier.Address;
            existingSupplier.IsActive = supplier.IsActive;
            existingSupplier.PaymentTerms = supplier.PaymentTerms;

            await SaveChanges();
            return Response(existingSupplier);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await GetEntityById<Supplier>(id);
            if (supplier == null) return Error("Supplier not found", HttpStatusCode.NotFound);

            await DeleteEntity(supplier);
            return Response(null, HttpStatusCode.NoContent);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        public async Task<IActionResult> GetSupplierStats()
        {
            var stats = new
            {
                totalSuppliers = await _context.Suppliers.CountAsync(),
                activeSuppliers = await _context.Suppliers.CountAsync(s => s.IsActive),
                totalPurchases = await _context.Purchases.CountAsync(),
                totalAmount = await _context.Purchases.SumAsync(p => p.TotalAmount)
            };

            return Response(stats);
        }
    }
}
