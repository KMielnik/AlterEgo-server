using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlterEgo.API.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IAnimationTaskRepository _repo;

        public HomeController(IAnimationTaskRepository repo)
        {
            _repo = repo;
        }

        [HttpGet, Route("Start")]
        public async Task<IActionResult> Index()
        {
            var user = new User("login", "password", "salt", "Agatka", "elo@wp.pl");
            var user2 = new User("login2", "password", "salt", "Agatka", "elo@wp.pl");
            var video = new DrivingVideo("a.mp4", user, TimeSpan.Zero);
            var video2 = new DrivingVideo("b.mp4", user2, TimeSpan.Zero);
            var image = new Image("a.jpg", user, TimeSpan.Zero);
            var image2 = new Image("b.jpg", user2, TimeSpan.Zero);
            var result = new ResultVideo("Output.mp4", user, TimeSpan.Zero);
            var result2 = new ResultVideo("outputto.mp4", user2, TimeSpan.Zero);

            var task1 = new AnimationTask(user, video, image, result);
            var task2 = new AnimationTask(user2, video2, image2, result2);

            await _repo.AddAsync(task1);
            await _repo.AddAsync(task2);
            if (task1==task2)
                return NotFound();
            return Ok((await _repo.GetAllAsync().ToListAsync()).Count);
        }
    }
}
