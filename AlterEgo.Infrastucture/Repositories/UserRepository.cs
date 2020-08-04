using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AlterEgoContext _context;

        public UserRepository(AlterEgoContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public IAsyncEnumerable<User> GetAllAsync()
            => _context
                .Users
                .ToAsyncEnumerable();

        public async Task<User> GetAsync(string login)
            => await GetAllAsync().FirstOrDefaultAsync(x => x.Login == login);

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
