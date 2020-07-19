using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Network
{
    public interface IAnimator
    {
        Task<AnimationTask> Animate(AnimationTask task);
    }
}
