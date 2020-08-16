using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Identity;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastructure.Services.Animation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AlterEgo.API.Controllers
{
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IImageManagerService _imageManagerService;
        private readonly IJWTService _jwtService;

        public HomeController(
            IImageManagerService imageManagerService, 
            IJWTService jwtService)
        {
            _imageManagerService = imageManagerService;
            _jwtService = jwtService;
        }

        [Authorize]
        [HttpPost, Route("GetAll")]
        public async Task<IActionResult> Index()
        {
            var login = GetAuthorizedUserLogin();

            return Ok(await _imageManagerService.GetAllActiveByUser(login).ToListAsync());
        }

        [Authorize]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> GetFile([FromQuery] string filename)
        {
            var login = GetAuthorizedUserLogin();

            return File(await _imageManagerService.GetFileStream(filename, login), "image/jpeg");
        }

        [Authorize]
        [HttpGet, Route("Refresh")]
        public async Task<IActionResult> Refresh([FromQuery] string filename)
        {
            var login = GetAuthorizedUserLogin();
            await _imageManagerService.Refresh(filename, login);
            return Ok();
        }

        [Authorize]
        [HttpPost, Route("Upload")]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var login = GetAuthorizedUserLogin();
            using (var inputStream = file.OpenReadStream())
            {
                await _imageManagerService.SaveFile(inputStream, file.FileName, login);
            }

            return Ok();
        }

        private string GetAuthorizedUserLogin()
            => _jwtService.GetLoginFromToken(HttpContext.User.Identity as ClaimsIdentity);
    }
}
