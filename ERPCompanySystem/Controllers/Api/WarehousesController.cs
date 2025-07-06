using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class WarehousesController : BaseController
    {
        public WarehousesController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Inventory")]
        public async Task<IActionResult> GetWarehouses()
        {
            var warehouses = await _context.Warehouses
                .Include(w => w.Products)
                .OrderBy(w => w.Name)
                .ToListAsync();
            return Response(warehouses);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Inventory")]
        public async Task<IActionResult> GetWarehouse(int id)
        {
            var warehouse = await _context.Warehouses
                .Include(w => w.Products)
                .FirstOrDefaultAsync(w => w.WarehouseId == id);
            
            if (warehouse == null) return Error("Warehouse not found", HttpStatusCode.NotFound);
            return Response(warehouse);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
        {
            if (!ModelState.IsValid) return Error("Invalid warehouse data");

            warehouse.CreatedAt = DateTime.UtcNow;
            
            _context.Warehouses.Add(warehouse);
            await SaveChanges();

            return Response(warehouse, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
        {
            if (id != warehouse.WarehouseId) return Error("ID mismatch");
            if (!ModelState.IsValid) return Error("Invalid warehouse data");

            var existingWarehouse = await GetEntityById<Warehouse>(id);
            if (existingWarehouse == null) return Error("Warehouse not found", HttpStatusCode.NotFound);

            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.Location = warehouse.Location;
            existingWarehouse.IsActive = warehouse.IsActive;

            await SaveChanges();
            return Response(existingWarehouse);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await GetEntityById<Warehouse>(id);
            if (warehouse == null) return Error("Warehouse not found", HttpStatusCode.NotFound);

            await DeleteEntity(warehouse);
            return Response(null, HttpStatusCode.NoContent);
        }

        [HttpGet("inventory/{id}")]
        [Authorize(Roles = "Admin,Manager,Inventory")]
        public async Task<IActionResult> GetWarehouseInventory(int id)
        {
            var inventory = await _context.Products
                .Where(p => p.WarehouseId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    p.CreatedAt
                })
                .ToListAsync();

            return Response(inventory);
        }
    }
}
