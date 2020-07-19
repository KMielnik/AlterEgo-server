using System;
using System.Text.RegularExpressions;
using AlterEgo.Core.Domains;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var user = new User("mk4e1446s", "passs", "salt", "supak", "elo@wp.pl");
                var image = new Image("dd", user, TimeSpan.FromMinutes(5));
                Console.WriteLine(image);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
