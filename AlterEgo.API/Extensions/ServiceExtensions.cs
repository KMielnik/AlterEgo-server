using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AlterEgo.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlterEgo.API.xml"));
                c.SwaggerDoc("v3", new OpenApiInfo
                {
                    Version = "v3",
                    Title = "AlterEgo - server",
                    Description = "This API allows queueing and storing animation tasks, that should be processed and returned to user.",
                    Contact = new OpenApiContact
                    {
                        Name = "KMielnik",
                        Email = "kamil.mielnik.km@gmail.com"
                    }
                });
            });

            return services;
        }
    }
}
