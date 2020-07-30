using AlterEgo.Core.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Repositories
{
    public interface IResultVideoRepository
    {
        Task<ResultVideo> GetAsync(string filename);
        IAsyncEnumerable<ResultVideo> GetAllAsync();
        Task AddAsync(ResultVideo resultVideo);
        Task UpdateAsunc(ResultVideo resultVideo);
        Task DeleteAsync(ResultVideo resultVideo);
    }
}
