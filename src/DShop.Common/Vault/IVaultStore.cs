using System.Collections.Generic;
using System.Threading.Tasks;

namespace DShop.Common.Vault
{
    public interface IVaultStore
    {
        Task<T> GetDefaultAsync<T>();
        Task<IDictionary<string, object>> GetDefaultAsync();
        Task<T> GetAsync<T>(string key);
        Task<IDictionary<string, object>> GetAsync(string key);
    }
}