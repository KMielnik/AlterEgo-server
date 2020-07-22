using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Services
{
    public interface IAnimator
    {
        Task<AnimationTask> Animate(AnimationTask task);
    }
}
