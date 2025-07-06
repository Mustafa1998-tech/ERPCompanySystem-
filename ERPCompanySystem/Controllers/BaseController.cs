using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;

namespace ERPCompanySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly AppDbContext _context;
        protected readonly ILogger<BaseController> _logger;

        public BaseController(AppDbContext context, ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected IActionResult Response<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return StatusCode((int)statusCode, new
            {
                success = statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created,
                data,
                timestamp = DateTime.UtcNow,
                message = statusCode == HttpStatusCode.OK ? "Success" : "Error"
            });
        }

        protected IActionResult Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return StatusCode((int)statusCode, new
            {
                success = false,
                message,
                timestamp = DateTime.UtcNow
            });
        }

        protected async Task<T?> GetEntityById<T>(int id) where T : class
        {
            return await _context.FindAsync<T>(id);
        }

        protected async Task<bool> EntityExists<T>(int id) where T : class
        {
            return await _context.FindAsync<T>(id) != null;
        }

        protected async Task<IActionResult> SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
                return Response(null, HttpStatusCode.OK);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error");
                return Error("Database update failed");
            }
        }

        protected async Task<IActionResult> DeleteEntity<T>(T entity) where T : class
        {
            try
            {
                _context.Remove(entity);
                await SaveChanges();
                return Response(null, HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete operation failed");
                return Error("Delete operation failed");
            }
        }

        protected string GetCurrentUserId()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        }
    }
}
