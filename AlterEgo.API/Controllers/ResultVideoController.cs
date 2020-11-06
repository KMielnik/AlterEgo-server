using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AlterEgo.Core.DTOs;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Identity;
using AlterEgo.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlterEgo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultVideoController : ControllerBase
    {
        private readonly IResultVideoManagerService _resultVideoManagerService;
        private readonly IJWTService _jwtService;

        public ResultVideoController(IResultVideoManagerService resultVideoManagerService, IJWTService jwtService)
        {
            _resultVideoManagerService = resultVideoManagerService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Gets list of infos of all logged users result videos.
        /// </summary>
        /// <param name="includeThumbnails">Indicate if you want thumbnails in response, if not then thumbnail will be null</param>
        /// <response code="200">Result videos list</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MediaFileInfo>), StatusCodes.Status200OK)]
        [HttpGet, Route("")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeThumbnails = false)
        {
            var login = GetAuthorizedUserLogin();

            var activeResultVideos = await _resultVideoManagerService.GetAllByUser(login, includeThumbnails).ToListAsync();

            return Ok(activeResultVideos);
        }

        /// <summary>
        /// Gets single media info of logged user.
        /// </summary>
        /// <param name="includeThumbnails">Indicate if you want thumbnail in response, if not then thumbnail will be null</param>
        /// <param name="filename">Filename of result video you want to download</param>
        /// <response code="404">File not found on server</response>
        /// <response code="200">Result videos media info</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MediaFileInfo>), StatusCodes.Status200OK)]
        [HttpGet, Route("{filename}")]
        public async Task<IActionResult> GetSingle(string filename, [FromQuery] bool includeThumbnails = false)
        {
            var login = GetAuthorizedUserLogin();

            var requestedMedia = await _resultVideoManagerService.GetSingle(login, includeThumbnails, filename);

            return Ok(requestedMedia);
        }

        /// <summary>
        /// Gets list of infos of all logged users active result videos.
        /// </summary>
        /// <param name="includeThumbnails">Indicate if you want thumbnails in response, if not then thumbnail will be null</param>
        /// <response code="200">Result videos list</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MediaFileInfo>), StatusCodes.Status200OK)]
        [HttpGet, Route("active")]
        public async Task<IActionResult> GetAllActive([FromQuery] bool includeThumbnails = false)
        {
            var login = GetAuthorizedUserLogin();

            var activeResultVideos = await _resultVideoManagerService.GetAllActiveByUser(login, includeThumbnails).ToListAsync();

            return Ok(activeResultVideos);
        }

        /// <summary>
        /// Gets result video in original resolution
        /// </summary>
        /// <param name="filename">Filename of result video you want to download</param>
        /// <response code="200">Full resolution result video</response>
        /// <response code="403">Logged user does not own this result video</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("image/jpeg")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet, Route("{filename}/file")]
        public async Task<IActionResult> GetOriginalResultVideo(string filename)
        {
            var login = GetAuthorizedUserLogin();

            var videoStream = await _resultVideoManagerService.GetFileStream(filename, login);

            var filetype = Path.GetExtension(filename) switch
            {
                ".mp4" => "video/mp4",
                string extension => throw new UnsupportedMediaTypeException(extension),
                _ => throw new ApplicationException($"File without exception exists in base: {filename}")
            };

            return File(videoStream, filetype);
        }



        /// <summary>
        /// Delete video from server
        /// </summary>
        /// <param name="filename">Filename of result video you want to delete.</param>
        /// <response code="200">File has been deleted</response>
        /// <response code="403">Logged user does not own this result video</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch, Route("{filename}/delete")]
        public async Task<IActionResult> Delete([Required] string filename)
        {
            var login = GetAuthorizedUserLogin();
            await _resultVideoManagerService.DeleteFile(filename, login);

            return Ok();
        }

        private string GetAuthorizedUserLogin()
           => _jwtService.GetLoginFromToken(HttpContext.User.Identity as ClaimsIdentity);
    }
}
