using AlterEgo.Core.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Repositories
{
    public interface IImageRepository
    {
        Task<Image> GetAsync(string filename);
        IAsyncEnumerable<Image> GetAllAsync();
        Task AddAsync(Image image);
    }
}
