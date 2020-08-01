using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AlterEgo.Infrastucture.Services;
using AlterEgo.Core.Interfaces;
using AlterEgo.Infrastucture.Contexts;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Repositories;
using AlterEgo.Core.Settings;

namespace AlterEgo.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IAnimator, CoreAnimator>();
            services.AddScoped<IAnimationTaskRepository, AnimationTaskRepository>();
            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<AlterEgoContext>();
            services.AddHostedService<AnimationTasksProcessorService>();

            services.Configure<EntityFrameworkSettings>(Configuration.GetSection("EnitityFrameworkSettings"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
