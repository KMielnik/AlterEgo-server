using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Repositories
{
    public interface IImageRepository
    {
        Task<Image> GetAsync(string filename);
        IAsyncEnumerable<Image> GetAllAsync();
        Task AddAsync(Image image);
    }
}
