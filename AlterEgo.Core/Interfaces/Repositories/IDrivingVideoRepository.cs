using AlterEgo.Core.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Repositories
{
    public interface IDrivingVideoRepository
    {
        Task<DrivingVideo> GetAsync(string filename);
        IAsyncEnumerable<DrivingVideo> GetAllAsync();
        Task AddAsync(DrivingVideo drivingVideo);
    }
}
