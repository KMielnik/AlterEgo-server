using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Animation
{
    public interface IUserNotifierService
    {
        Task NotifyUserAboutFinishedTask(User user, AnimationTask task);
    }
}
