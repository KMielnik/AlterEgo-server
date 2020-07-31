using AlterEgo.Core.Domains;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastucture.Exceptions;
using AlterEgo.Infrastucture.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var loggerFactory = CreateLoggerFactory();
            var coreAnimatorlogger = loggerFactory.CreateLogger<CoreAnimator>();

            try
            {
                var settings = new CoreAnimatorSettings
                {
                    IsUsingDocker = false,
                    PythonStartingPoint = "python/run.py",
                    ImagesDirectory = "files/images",
                    VideosDirectory = "files/videos",
                    TempDirectory = "files/temp",
                    OutputDirectory = "files/output",
                    UsingGPU = false,
                };
                var animator = new CoreAnimator(Options.Create(settings), coreAnimatorlogger);

                var user = new User("login", "password", "salt", "Agatka", "elo@wp.pl");
                var video = new DrivingVideo("a.mp4", user, TimeSpan.Zero);
                var image = new Image("a.jpg", user, TimeSpan.Zero);
                var image2 = new Image("a.jpg", user, TimeSpan.Zero);
                var result = new ResultVideo("Output.mp4", user, TimeSpan.Zero);
                var result2 = new ResultVideo("outputto.mp4", user, TimeSpan.Zero);

                var task1 = new AnimationTask(user, video, image, result);
                var task2 = new AnimationTask(user, video, image2, result2);

                await animator.Animate(task1);

                Console.WriteLine(task1.Status);
                
            }
            catch (ProcessingAnimationFailedException ex)
            {
                coreAnimatorlogger.LogError(ex, "Error wehen processing animation");
            }

            Log.CloseAndFlush();
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            var serilogLogger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:53411/")
                .CreateLogger();

            Log.Logger = serilogLogger;

            return new LoggerFactory()
                .AddSerilog(serilogLogger);
        }
    }
}
