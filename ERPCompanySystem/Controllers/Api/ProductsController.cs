using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class ProductsController : BaseController
    {
        public ProductsController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Warehouse)
                .ToListAsync();
            return Response(products);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await GetEntityById<Product>(id);
            if (product == null) return Error("Product not found", HttpStatusCode.NotFound);
            return Response(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid) return Error("Invalid product data");

            product.CreatedAt = DateTime.UtcNow;
            
            _context.Products.Add(product);
            await SaveChanges();

            return Response(product, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id) return Error("ID mismatch");
            if (!ModelState.IsValid) return Error("Invalid product data");

            var existingProduct = await GetEntityById<Product>(id);
            if (existingProduct == null) return Error("Product not found", HttpStatusCode.NotFound);

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.WarehouseId = product.WarehouseId;

            await SaveChanges();
            return Response(existingProduct);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await GetEntityById<Product>(id);
            if (product == null) return Error("Product not found", HttpStatusCode.NotFound);

            await DeleteEntity(product);
            return Response(null, HttpStatusCode.NoContent);
        }
    }
}
