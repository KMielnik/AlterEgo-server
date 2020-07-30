using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Repositories
{
    public interface IAnimationTaskRepository
    {
        Task<AnimationTask> GetAsync(Guid id);
        IAsyncEnumerable<AnimationTask> GetAllAsync();
        Task AddAsync(AnimationTask animationTask);
        Task UpdateAsync(AnimationTask animationTask);
        Task DeleteAsync(AnimationTask animationTask);
    }
}
