using AlterEgo.Core.Interfaces;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastucture.Contexts;
using AlterEgo.Infrastucture.Exceptions;
using AlterEgo.Infrastucture.Repositories;
using AlterEgo.Infrastucture.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.Configure<AnimationTaskProcessorSettings>(
                configuration
                    .GetSection("AnimationProcessing"));

            #endregion

            #region repositories
            services
                .AddScoped<IAnimationTaskRepository, AnimationTaskRepository>()
                .AddScoped<IImageRepository, ImageRepository>()
                .AddScoped<IResultVideoRepository, ResultVideoRepository>()
                .AddScoped<IDrivingVideoRepository, DrivingVideoRepository>()
                .AddScoped<IUserRepository, UserRepository>();

            #endregion

            var animator = configuration
                .GetSection("AnimationProcessing")
                .GetSection("Animator")
                .Value;

            switch (animator)
            {
                case "CoreAnimator":
                    services.AddSingleton<IAnimator, CoreAnimator>();
                    break;
                default:
                    throw new MissingConfigurationSetting("Animator", "AnimationProcessing");
            }

            services.AddDbContext<AlterEgoContext>();

            services.AddHostedService<AnimationTasksProcessorService>();
        }
    }
}
