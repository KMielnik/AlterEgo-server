using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlterEgo.Core.Domains;
using AlterEgo.Infrastucture.Services;

namespace Tester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var animator = new Animator();

                var user = new User("login", "password", "salt", "Agatka", "elo@wp.pl");
                var video = new DrivingVideo("a.mp4", user, TimeSpan.Zero);
                var image = new Image("a.jpg", user, TimeSpan.Zero);
                var image2 = new Image("a.jpg", user, TimeSpan.Zero);
                var result = new ResultVideo("Output.mp4", user, TimeSpan.Zero);
                var result2 = new ResultVideo("outputto.mp4", user, TimeSpan.Zero);

                var task1 = new AnimationTask(user, video, image, result);
                var task2 = new AnimationTask(user, video, image2, result2);

                await foreach (var task in animator.Animate(task1,task2))
                {
                    Console.WriteLine(task.SourceImage);
                }
            }
            catch
            {
                Console.WriteLine("Eee");
            }
        }
    }
}
