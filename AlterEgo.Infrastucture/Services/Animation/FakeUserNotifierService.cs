using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Animation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Animation
{
    class FakeUserNotifierService : IUserNotifierService
    {
        private readonly ILogger<FakeUserNotifierService> _logger;

        public FakeUserNotifierService(ILogger<FakeUserNotifierService> logger)
        {
            _logger = logger;
        }

        public async Task NotifyUserAboutFinishedTask(User user, AnimationTask task)
        {
            _logger.LogDebug("Simulating notyfing {@User} about finished {@Task}.");

            task.SetStatusNotified();

            await Task.CompletedTask;
        }
    }
}
