using Microsoft.AspNetCore.Mvc;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Models.Inventory;
using System.Security.Claims;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CustomAuthorize(new string[] { "Manager", "Admin" })]  // Only Manager and Admin can access
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Inventory/Stock
        [HttpGet("Stock")]
        public async Task<ActionResult<IEnumerable<Product>>> GetStock()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Inventory/Movements
        [HttpGet("Movements")]
        public async Task<ActionResult<IEnumerable<StockMovement>>> GetStockMovements()
        {
            return await _context.StockMovements
                .Include(m => m.Product)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();
        }

        // POST: api/Inventory/AdjustStock
        [HttpPost("AdjustStock")]
        public async Task<ActionResult> AdjustStock([FromBody] StockAdjustmentRequest request)
        {
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            var movement = new StockMovement
            {
                ProductId = request.ProductId,
                MovementDate = DateTime.Now,
                MovementType = "ADJUSTMENT",
                Quantity = request.Quantity,
                ReferenceNumber = request.ReferenceNumber,
                Description = request.Description,
                CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System"
            };

            _context.StockMovements.Add(movement);
            product.StockQuantity += request.Quantity;
            _context.Products.Update(product);

            await _context.SaveChangesAsync();
            return Ok(movement);
        }
    }

    public class StockAdjustmentRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
