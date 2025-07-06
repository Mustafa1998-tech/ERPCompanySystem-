using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class PurchasesController : BaseController
    {
        public PurchasesController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        public async Task<IActionResult> GetPurchases()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
                .ToListAsync();
            return Response(purchases);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        public async Task<IActionResult> GetPurchase(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                .ThenInclude(pd => pd.Product)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
            
            if (purchase == null) return Error("Purchase not found", HttpStatusCode.NotFound);
            return Response(purchase);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Purchase")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreatePurchase([FromBody] Purchase purchase)
        {
            if (!ModelState.IsValid) return Error("Invalid purchase data");

            purchase.CreatedAt = DateTime.UtcNow;
            
            _context.Purchases.Add(purchase);
            await SaveChanges();

            return Response(purchase, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] Purchase purchase)
        {
            if (id != purchase.PurchaseId) return Error("ID mismatch");
            if (!ModelState.IsValid) return Error("Invalid purchase data");

            var existingPurchase = await GetEntityById<Purchase>(id);
            if (existingPurchase == null) return Error("Purchase not found", HttpStatusCode.NotFound);

            existingPurchase.SupplierId = purchase.SupplierId;
            existingPurchase.TotalAmount = purchase.TotalAmount;
            existingPurchase.Status = purchase.Status;
            existingPurchase.PurchaseDetails = purchase.PurchaseDetails;

            await SaveChanges();
            return Response(existingPurchase);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var purchase = await GetEntityById<Purchase>(id);
            if (purchase == null) return Error("Purchase not found", HttpStatusCode.NotFound);

            await DeleteEntity(purchase);
            return Response(null, HttpStatusCode.NoContent);
        }
    }
}
