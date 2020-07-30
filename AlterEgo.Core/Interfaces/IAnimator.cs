using AlterEgo.Core.Domains;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces
{
    public interface IAnimator
    {
        Task Animate(AnimationTask task);
    }
}
