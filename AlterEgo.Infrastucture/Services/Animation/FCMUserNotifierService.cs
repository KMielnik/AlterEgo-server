using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Animation;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Animation
{
    class FCMUserNotifierService : IUserNotifierService
    {
        private readonly ILogger<FakeUserNotifierService> _logger;

        public FCMUserNotifierService(ILogger<FakeUserNotifierService> logger)
        {
            _logger = logger;
        }

        public async Task NotifyUserAboutFinishedTask(User user, AnimationTask task)
        {
            _logger.LogDebug("Notyfing {@User} about finished {@Task}.");

            var topic = user.Login;

            var message = new Message
            {
                Data = new Dictionary<string, string>{
                    {"task_id",task.Id.ToString()},
                    {"click_action", "FLUTTER_NOTIFICATION_CLICK"}
                },
                Notification = new Notification
                {
                    Title = "Task finished",
                    Body = "Click to open the app"
                },
                Topic = topic,
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            _logger.LogDebug("FCM notification response: {@Response}", response);

            task.SetStatusNotified();

        }
    }
}
