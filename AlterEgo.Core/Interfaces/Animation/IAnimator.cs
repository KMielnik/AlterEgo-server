using AlterEgo.Core.Domains;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Animation
{
    public interface IAnimator
    {
        Task Animate(AnimationTask task);
    }
}
