using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Repositories
{
    public interface IDrivingVideoRepository
    {
        Task<DrivingVideo> GetAsync(string filename);
        IAsyncEnumerable<DrivingVideo> GetAllAsync();
        Task AddAsync(DrivingVideo drivingVideo);
    }
}
