using Microsoft.AspNetCore.Mvc;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using System.Security.Claims;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CustomAuthorize(new string[] { "User", "Manager", "Admin" })]  // All roles can access
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
        {
            return await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.Product)
                .ToListAsync();
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Client)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }
            return sale;
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale(Sale sale)
        {
            // Update product stock
            var product = await _context.Products.FindAsync(sale.ProductId);
            if (product == null || product.StockQuantity < sale.Quantity)
            {
                return BadRequest("Product not found or insufficient stock");
            }

            product.StockQuantity -= sale.Quantity;
            _context.Products.Update(product);

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }

        // PUT: api/Sales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, Sale sale)
        {
            if (id != sale.Id)
            {
                return BadRequest();
            }

            _context.Entry(sale).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            // Update product stock back
            var product = await _context.Products.FindAsync(sale.ProductId);
            if (product != null)
            {
                product.StockQuantity += sale.Quantity;
                _context.Products.Update(product);
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
