using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ERPCompanySystem.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder = "uploads");
        Task DeleteFileAsync(string filePath);
        Task<bool> FileExistsAsync(string filePath);
        Task<byte[]> GetFileAsync(string filePath);
        Task<string> GenerateTemporaryUrlAsync(string filePath, int minutesToExpire = 60);
    }
}
