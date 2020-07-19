using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Repositories
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
