using System;
using System.IO;
using System.Text.RegularExpressions;
using AlterEgo.Core.Domains;
using AlterEgo.Core.Helpers;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var user = new User("login", "password", "salt", "Agatka", "elo@wp.pl");
                var video = new DrivingVideo("elo.mp4", user, TimeSpan.Zero);
                var image = new Image("aa.jpg", user, TimeSpan.Zero);
                var image2 = new Image("aa2.jpg", user, TimeSpan.Zero);
                var result = new ResultVideo("output.mp4", user, TimeSpan.Zero);
                var result2 = new ResultVideo("outputto.mp4", user, TimeSpan.Zero);

                var elo = AnimationCommandBuilder.UsingDocker("kamilmielnik/alterge")
                    .WithExecutablePath("python3")
                    .WithImagesDirectory("../images")
                    .WithVideosDirectory("videos")
                    .WithTempDirectory("temp")
                    .WithOutputDirectory("C:\\New folder")
                    .WithParameters()
                    .WithDrivingVideo(video)
                    .AddResultAnimation(image, result)
                    .AddResultAnimation(image2, result2)
                    .WithAudio()
                    .WithGPUSupport()
                    .Build();

                Console.WriteLine($"{elo.path} - {elo.arguments}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
