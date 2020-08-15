using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastructure.Services.Animation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlterEgo.API.Controllers
{
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IImageManagerService _imageManagerService;

        public HomeController(IImageManagerService imageManagerService)
        {
            _imageManagerService = imageManagerService;
        }

        [Authorize]
        [HttpPost, Route("GetAll")]
        public async Task<IActionResult> Index()
        {

            return Ok(await _imageManagerService.GetAllActiveByUser("login123").ToListAsync());
        }

        [Authorize]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> GetFile([FromQuery] string filename)
        {

            return File(await _imageManagerService.GetFileStream(filename, "login123"), "image/jpeg");
        }

        [Authorize]
        [HttpGet, Route("Refresh")]
        public async Task<IActionResult> Refresh([FromQuery] string filename)
        {
            await _imageManagerService.Refresh(filename, "login123");
            return Ok();
        }

        [Authorize]
        [HttpPost, Route("Upload")]
        public async Task<IActionResult> Index(IFormFile file)
        {
            using(var inputStream = file.OpenReadStream())
            {
                await _imageManagerService.SaveFile(inputStream, file.FileName, "login123");
            }

            return Ok();
        }
    }
}
