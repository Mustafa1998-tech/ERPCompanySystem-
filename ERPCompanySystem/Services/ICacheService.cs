using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace ERPCompanySystem.Services
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task ClearAsync();
    }
}
