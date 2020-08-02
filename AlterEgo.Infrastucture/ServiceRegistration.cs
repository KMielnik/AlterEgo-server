using AlterEgo.Core.Interfaces;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastucture.Contexts;
using AlterEgo.Infrastucture.Repositories;
using AlterEgo.Infrastucture.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture
{
    public static class ServiceRegistration
    {
        public static void AddAlterEgoInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region settings
            services.Configure<EntityFrameworkSettings>(
                configuration
                    .GetSection("EnitityFrameworkSettings"));

            services.Configure<CoreAnimatorSettings>(
                configuration
                    .GetSection("AnimationProcessing")
                    .GetSection("CoreAnimator"));

            #endregion

            #region repositories
            services.AddScoped<IAnimationTaskRepository, AnimationTaskRepository>();
            #endregion

            services.AddSingleton<IAnimator, CoreAnimator>();

            services.AddDbContext<AlterEgoContext>();

            services.AddHostedService<AnimationTasksProcessorService>();
        }
    }
}
