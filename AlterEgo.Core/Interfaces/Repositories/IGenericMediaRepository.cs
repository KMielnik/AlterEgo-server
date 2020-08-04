using AlterEgo.Core.Domains.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Repositories
{
    public interface IGenericMediaRepository<T> where T : MediaResource
    {
        Task<T> GetAsync(string filename);
        IAsyncEnumerable<T> GetAllAsync();
        Task<T> AddAsync(T mediaEntity);
        Task UpdateAsync(T mediaEntity);
        Task DeleteAsync(T mediaEntity);
    }
}
