using AlterEgo.Core.Interfaces;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastructure.Contexts;
using AlterEgo.Infrastructure.Exceptions;
using AlterEgo.Infrastructure.Repositories;
using AlterEgo.Infrastructure.Services;
using AlterEgo.Infrastructure.Services.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlterEgo.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddAlterEgoInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region settings
            services.Configure<EntityFrameworkSettings>(
                configuration
                    .GetSection("EnitityFrameworkSettings"));

            services.Configure<JWTSettings>(
                configuration.GetSection("JWT"));

            var animationProcessingSection = configuration
                .GetSection("AnimationProcessing");

            services.Configure<AnimationTaskProcessorSettings>(
                animationProcessingSection);

            services.Configure<CoreAnimatorSettings>(
                animationProcessingSection
                    .GetSection("CoreAnimator"));

            services.Configure<FakeAnimatorSettings>(
                animationProcessingSection
                    .GetSection("FakeAnimator"));
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
                case "FakeAnimator":
                    services.AddSingleton<IAnimator, FakeAnimator>();
                    break;
                default:
                    throw new MissingConfigurationSetting("Animator", "AnimationProcessing");
            }

            services.AddSingleton<IJWTService, JWTService>();
            services.AddSingleton<IEncrypter, Encrypter>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddDbContext<AlterEgoContext>();

            services.AddHostedService<AnimationTasksProcessorService>();
        }
    }
}
