using AlterEgo.Core.Domains;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastucture.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var settings = new CoreAnimatorSettings
                {
                    IsUsingDocker = true,
                    DockerImage = "kamilmielnik/alterego-core:2.0.4",
                    ImagesDirectory = "files/images",
                    VideosDirectory = "files/videos",
                    TempDirectory = "files/temp",
                    OutputDirectory = "files/output",
                    UsingGPU = false,
                };
                var animator = new CoreAnimator(Options.Create(settings));

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
            catch (Exception ex)
            {
                Console.WriteLine("Eee");
            }
        }
    }
}
