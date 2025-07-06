using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClientsController : BaseController
    {
        public ClientsController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _context.Clients
                .OrderBy(c => c.Name)
                .ToListAsync();
            return Response(clients);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Sales)
                .FirstOrDefaultAsync(c => c.ClientId == id);
            
            if (client == null) return Error("Client not found", HttpStatusCode.NotFound);
            return Response(client);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Sales")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            if (!ModelState.IsValid) return Error("Invalid client data");

            client.CreatedAt = DateTime.UtcNow;
            
            _context.Clients.Add(client);
            await SaveChanges();

            return Response(client, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Sales")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (id != client.ClientId) return Error("ID mismatch");
            if (!ModelState.IsValid) return Error("Invalid client data");

            var existingClient = await GetEntityById<Client>(id);
            if (existingClient == null) return Error("Client not found", HttpStatusCode.NotFound);

            existingClient.Name = client.Name;
            existingClient.Email = client.Email;
            existingClient.Phone = client.Phone;
            existingClient.Address = client.Address;
            existingClient.IsActive = client.IsActive;

            await SaveChanges();
            return Response(existingClient);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await GetEntityById<Client>(id);
            if (client == null) return Error("Client not found", HttpStatusCode.NotFound);

            await DeleteEntity(client);
            return Response(null, HttpStatusCode.NoContent);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Manager,Sales")]
        public async Task<IActionResult> SearchClients(string term = "", int page = 1, int pageSize = 10)
        {
            var query = _context.Clients
                .Where(c => c.Name.Contains(term) || 
                           c.Email.Contains(term) || 
                           c.Phone.Contains(term));

            var total = await query.CountAsync();
            var clients = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Response(new
            {
                data = clients,
                total,
                page,
                pageSize
            });
        }
    }
}
