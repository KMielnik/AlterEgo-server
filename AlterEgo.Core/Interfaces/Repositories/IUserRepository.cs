using AlterEgo.Core.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(string login);
        IAsyncEnumerable<User> GetAllAsync();
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
