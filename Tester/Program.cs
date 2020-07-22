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
                await animator.Animate(null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
