using AlterEgo.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Animation
{
    public interface IAnimationTaskService
    {
        Task<AnimationTaskDTO> AddNewTask(AnimationTaskRequest request, string userLogin);
        IAsyncEnumerable<AnimationTaskDTO> GetAll(string userLogin);
        Task<AnimationTaskDTO> GetSpecificTask(string id, string userLogin);
    }
}
