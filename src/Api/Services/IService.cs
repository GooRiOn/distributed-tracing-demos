using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Api.Services
{
    public interface IService
    {
        [Get("/api/values")]
        Task<IEnumerable<string>> GetValuesAsync();
    }
}