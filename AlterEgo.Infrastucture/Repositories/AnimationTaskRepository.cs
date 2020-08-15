using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Repositories
{
    public class AnimationTaskRepository : IAnimationTaskRepository
    {
        private readonly AlterEgoContext _context;

        public AnimationTaskRepository(AlterEgoContext context)
        {
            _context = context;
        }

        public async Task<AnimationTask> AddAsync(AnimationTask animationTask)
        {
            await _context.AnimationTasks.AddAsync(animationTask);
            await _context.SaveChangesAsync();

            return animationTask;
        }

        public async Task DeleteAsync(AnimationTask animationTask)
        {
            _context.AnimationTasks.Remove(animationTask);
            await _context.SaveChangesAsync();
        }

        public IAsyncEnumerable<AnimationTask> GetAllAsync()
            => _context.AnimationTasks
                .Include(x => x.Owner)
                .Include(x => x.SourceVideo)
                .Include(x => x.SourceImage)
                .Include(x => x.ResultAnimation)
                .AsAsyncEnumerable();

        public async Task<AnimationTask> GetAsync(Guid id)
            => await GetAllAsync()
                .SingleOrDefaultAsync(x => x.Id == id);

        public async Task UpdateAsync(AnimationTask animationTask)
        {
            _context.AnimationTasks.Update(animationTask);
            await _context.SaveChangesAsync();
        }
    }
}
