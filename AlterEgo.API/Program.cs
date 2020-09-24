using AlterEgo.Core.Settings;
using AlterEgo.Infrastructure.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;

namespace AlterEgo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            try
            {
                Log.Information("Web host started");

                var host = CreateHostBuilder(args).Build();
                    
                using(var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AlterEgoContext>();
                    db.Database.Migrate();
                }

                InitializeFileDirectories(config.GetSection("FilesLocationSettings").Get<FilesLocationSettings>());

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminaded unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void InitializeFileDirectories(FilesLocationSettings settings)
        {
            Directory.CreateDirectory(settings.ImagesDirectory);
            Directory.CreateDirectory(settings.VideosDirectory);
            Directory.CreateDirectory(settings.OutputDirectory);
            Directory.CreateDirectory(settings.TempDirectory);
        }
    }
}
