using ERPCompanySystem.Data;
using ERPCompanySystem.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.PasswordHash = HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(int id, User user)
        {
            var existingUser = await GetUserByIdAsync(id);
            if (existingUser == null) return null;

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            if (!VerifyPassword(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Enable2FAAsync(int userId, string secret)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            user.TwoFactorSecret = secret;
            user.Is2FAEnabled = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Disable2FAAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            user.TwoFactorSecret = null;
            user.Is2FAEnabled = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Verify2FAAsync(int userId, string code)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null || !user.Is2FAEnabled) return false;

            // TODO: Implement 2FA verification logic
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var inputHash = HashPassword(password);
            return inputHash == hash;
        }
    }
}
