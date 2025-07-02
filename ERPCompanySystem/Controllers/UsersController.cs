using Microsoft.AspNetCore.Mvc;
using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using ERPCompanySystem.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Net;

namespace ERPCompanySystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[CustomAuthorize(new string[] { "Admin" })] // Only admins can manage users
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(AppDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.Role,
                    u.CreatedAt,
                    u.LastLogin
                })
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving users");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
        {
            return Conflict("Username already exists");
        }

        user.Password = _context.AuthenticationService.HashPassword(user.Password);
        user.CreatedAt = DateTime.Now;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(user.Password))
        {
            existingUser.Password = _context.AuthenticationService.HashPassword(user.Password);
        }

        existingUser.Username = user.Username;
        existingUser.FullName = user.FullName;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Role = user.Role;

        try
        {
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UserExists(id))
            {
                return NotFound();
            }
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> UserExists(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
}
