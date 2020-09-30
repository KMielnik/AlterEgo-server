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
    public class DrivingVideoController : ControllerBase
    {
        private readonly IDrivingVideoManagerService _drivingVideoManagerService;
        private readonly IJWTService _jwtService;

        public DrivingVideoController(IDrivingVideoManagerService drivingVideoManagerService, IJWTService jwtService)
        {
            _drivingVideoManagerService = drivingVideoManagerService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Gets list of infos of all logged users driving videos.
        /// </summary>
        /// <param name="includeThumbnails">Indicate if you want thumbnails in response, if not then thumbnail will be null</param>
        /// <response code="200">Driving videos list</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MediaFileInfo>), StatusCodes.Status200OK)]
        [HttpGet, Route("")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeThumbnails = false)
        {
            var login = GetAuthorizedUserLogin();

            var activeDrivingVideos = await _drivingVideoManagerService.GetAllActiveByUser(login, includeThumbnails).ToListAsync();

            return Ok(activeDrivingVideos);
        }

        /// <summary>
        /// Gets drivingVideo in original resolution
        /// </summary>
        /// <param name="filename">Filename of driving video you want to download</param>
        /// <response code="200">Full resolution driving video</response>
        /// <response code="403">Logged user does not own this driving video</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("image/jpeg")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet, Route("{filename}")]
        public async Task<IActionResult> GetOriginalDrivingVideo(string filename)
        {
            var login = GetAuthorizedUserLogin();

            var videoStream = await _drivingVideoManagerService.GetFileStream(filename, login);

            var filetype = Path.GetExtension(filename) switch
            {
                ".mp4" => "video/mp4",
                string extension => throw new UnsupportedMediaTypeException(extension),
                _ => throw new ApplicationException($"File without exception exists in base: {filename}")
            };

            return File(videoStream, filetype);
        }

        /// <summary>
        /// Refreshes lifetime of driving video
        /// </summary>
        /// <param name="filename">Filename of driving video you want to refresh</param>
        /// <response code="200">Lifetime refreshed</response>
        /// <response code="403">Logged user does not own this driving video</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MediaFileInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch, Route("{filename}/refresh")]
        public async Task<IActionResult> Refresh(string filename)
        {
            var login = GetAuthorizedUserLogin();
            var updatedVideo = await _drivingVideoManagerService.Refresh(filename, login);

            return Ok(updatedVideo);
        }

        /// <summary>
        /// Uploads new driving video
        /// </summary>
        /// <remarks>Allows only .mp4</remarks>
        /// <param name="file">Video in .mp4 format to be uploaded</param>
        /// <response code="201">File has been created</response>
        /// <response code="415">Only .mp4 extension is allowed</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MediaFileInfo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost, Route("")]
        public async Task<IActionResult> Upload([Required] IFormFile file)
        {
            if (file is null)
                return StatusCode(415, "Driving video musn't be null");

            var extension = Path.GetExtension(file.FileName);
            if (extension != ".mp4")
                return StatusCode(415, "Driving video must be in .mp4 format");


            var login = GetAuthorizedUserLogin();

            using var inputStream = file.OpenReadStream();
            var newVideoInfo = await _drivingVideoManagerService.SaveFile(inputStream, file.FileName, login);

            return CreatedAtAction(nameof(GetOriginalDrivingVideo), new { filename = newVideoInfo.Filename }, newVideoInfo);
        }

        /// <summary>
        /// Delete video from server
        /// </summary>
        /// <param name="filename">Filename of driving video you want to delete.</param>
        /// <response code="200">File has been deleted</response>
        /// <response code="403">Logged user does not own this driving video</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch, Route("{filename}/delete")]
        public async Task<IActionResult> Delete([Required] string filename)
        {
            var login = GetAuthorizedUserLogin();
            await _drivingVideoManagerService.DeleteFile(filename, login);

            return Ok();
        }

        private string GetAuthorizedUserLogin()
            => _jwtService.GetLoginFromToken(HttpContext.User.Identity as ClaimsIdentity);
    }
}
