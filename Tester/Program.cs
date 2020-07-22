using System;
using System.Diagnostics;
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
                var video = new DrivingVideo("a.mp4", user, TimeSpan.Zero);
                var image = new Image("a.jpg", user, TimeSpan.Zero);
                var image2 = new Image("a.jpg", user, TimeSpan.Zero);
                var result = new ResultVideo("Output.mp4", user, TimeSpan.Zero);
                var result2 = new ResultVideo("outputto.mp4", user, TimeSpan.Zero);

                var elo = AnimationCommandBuilder.UsingPython("python/run.py")
                    .WithExecutablePath()
                    .WithImagesDirectory("images")
                    .WithVideosDirectory("videos")
                    .WithTempDirectory("temp")
                    .WithOutputDirectory("out put")
                    .WithParameters()
                    .WithDrivingVideo(video)
                    .AddResultAnimation(image, result)
                    .AddResultAnimation(image2, result2)
                    .WithAudio()
                    .Build();

                Console.WriteLine($"{elo.path} {elo.arguments}");

                Process.Start(elo.path, elo.arguments).WaitForExit();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
