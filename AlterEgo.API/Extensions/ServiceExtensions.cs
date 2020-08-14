using AlterEgo.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlterEgo.Infrastucture.xml"));
                c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlterEgo.Core.xml"));

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

                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<AuthenticationRequirementsOperationFilter>();

                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token as - Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer",
                            },
                            Scheme = "bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });

            return services;
        }

        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, JWTSettings settings)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = settings.Issuer,
                        ValidAudience = settings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        private class AuthenticationRequirementsOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var isAuthorized = (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                                && !context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()) //this excludes controllers with AllowAnonymous attribute in case base controller has Authorize attribute
                                || (context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                                && !context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()); // this excludes methods with AllowAnonymous attribute

                if (!isAuthorized)
                    return;

                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security ??= new List<OpenApiSecurityRequirement>();

                var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" } };
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = new List<string>()
                });
            }
        }
    }
}
