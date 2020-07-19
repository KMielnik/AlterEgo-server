using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Repositories
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
