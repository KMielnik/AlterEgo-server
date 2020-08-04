using AlterEgo.Core.Domains.Common;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Repositories
{
    public class GenericMediaRepository<T> : IGenericMediaRepository<T> where T : MediaResource
    {
        private readonly AlterEgoContext _context;

        public GenericMediaRepository(AlterEgoContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T mediaEntity)
        {
            await _context.Set<T>().AddAsync(mediaEntity);
            await _context.SaveChangesAsync();

            return mediaEntity;
        }

        public async Task DeleteAsync(T mediaEntity)
        {
            _context.Set<T>().Remove(mediaEntity);
            await _context.SaveChangesAsync();
        }

        public IAsyncEnumerable<T> GetAllAsync()
            => _context
                .Set<T>()
                .Include(x => x.Owner)
                .ToAsyncEnumerable();

        public async Task<T> GetAsync(string filename)
            => await GetAllAsync().FirstOrDefaultAsync(x => x.Filename == filename);

        public async Task UpdateAsync(T mediaEntity)
        {
            _context.Entry(mediaEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
