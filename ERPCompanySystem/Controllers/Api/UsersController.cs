using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;
using System.Security.Claims;

namespace ERPCompanySystem.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        public UsersController(AppDbContext context, ILogger<BaseController> logger) : base(context, logger) { }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
            return Response(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await GetEntityById<User>(id);
            if (user == null) return Error("User not found", HttpStatusCode.NotFound);
            return Response(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid) return Error("Invalid user data");

            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            
            _context.Users.Add(user);
            await SaveChanges();

            return Response(user, HttpStatusCode.Created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id) return Error("ID mismatch");
            if (!ModelState.IsValid) return Error("Invalid user data");

            var existingUser = await GetEntityById<User>(id);
            if (existingUser == null) return Error("User not found", HttpStatusCode.NotFound);

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;

            await SaveChanges();
            return Response(existingUser);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [CustomAuthorize.RequireJwt]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await GetEntityById<User>(id);
            if (user == null) return Error("User not found", HttpStatusCode.NotFound);

            await DeleteEntity(user);
            return Response(null, HttpStatusCode.NoContent);
        }
    }
}
