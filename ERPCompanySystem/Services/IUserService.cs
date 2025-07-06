using ERPCompanySystem.Models;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> Enable2FAAsync(int userId, string secret);
        Task<bool> Disable2FAAsync(int userId);
        Task<bool> Verify2FAAsync(int userId, string code);
    }
}
