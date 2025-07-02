using Microsoft.AspNetCore.Mvc;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using System.Security.Claims;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CustomAuthorize(new string[] { "Manager", "Admin" })]  // Only Manager and Admin can access
    public class PurchasesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchasesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Purchases
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            return await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .ToListAsync();
        }

        // GET: api/Purchases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }
            return purchase;
        }

        // POST: api/Purchases
        [HttpPost]
        public async Task<ActionResult<Purchase>> CreatePurchase(Purchase purchase)
        {
            // Update product stock
            var product = await _context.Products.FindAsync(purchase.ProductId);
            if (product == null)
            {
                return BadRequest("Product not found");
            }

            product.StockQuantity += purchase.Quantity;
            _context.Products.Update(product);

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPurchase), new { id = purchase.Id }, purchase);
        }

        // PUT: api/Purchases/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchase).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        // DELETE: api/Purchases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            // Update product stock back
            var product = await _context.Products.FindAsync(purchase.ProductId);
            if (product != null)
            {
                product.StockQuantity -= purchase.Quantity;
                _context.Products.Update(product);
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
