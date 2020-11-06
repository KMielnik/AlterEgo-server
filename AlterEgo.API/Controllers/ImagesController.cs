using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
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
    public class ImagesController : ControllerBase
    {
        private readonly IImageManagerService _imageManagerService;
        private readonly IJWTService _jwtService;

        public ImagesController(IImageManagerService imageManagerService, IJWTService jwtService)
        {
            _imageManagerService = imageManagerService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Gets list of infos of all logged users active images.
        /// </summary>
        /// <param name="includeThumbnails">Indicate if you want thumbnails in response, if not then thumbnail will be null</param>
        /// <response code="200">Images list</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MediaFileInfo>), StatusCodes.Status200OK)]
        [HttpGet, Route("active")]
        public async Task<IActionResult> GetAllActive([FromQuery] bool includeThumbnails = false)
        {
            var login = GetAuthorizedUserLogin();

            var activeImages = await _imageManagerService.GetAllActiveByUser(login, includeThumbnails).ToListAsync();

            return Ok(activeImages);
        }

        /// <summary>
        /// Gets list of infos of all logged users images.
        /// </summary>
        /// <param name="includeThumbnails">Indicate if you want thumbnails in response, if not then thumbnail will be null</param>
        /// <response code="200">Images list</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MediaFileInfo>), StatusCodes.Status200OK)]
        [HttpGet, Route("")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeThumbnails = false)
        {
            var login = GetAuthorizedUserLogin();

            var activeImages = await _imageManagerService.GetAllByUser(login, includeThumbnails).ToListAsync();

            return Ok(activeImages);
        }

        /// <summary>
        /// Gets single media info of logged user.
        /// </summary>
        /// <param name="includeThumbnails">Indicate if you want thumbnail in response, if not then thumbnail will be null</param>
        /// <param name="filename">Filename of image you want to download</param>
        /// <response code="404">File not found on server</response>
        /// <response code="200">Image media info</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<MediaFileInfo>), StatusCodes.Status200OK)]
        [HttpGet, Route("{filename}")]
        public async Task<IActionResult> GetSingle(string filename, [FromQuery] bool includeThumbnails = false)
        {
            var login = GetAuthorizedUserLogin();

            var requestedMedia = await _imageManagerService.GetSingle(login, includeThumbnails, filename);

            return Ok(requestedMedia);
        }

        /// <summary>
        /// Gets image in original resolution
        /// </summary>
        /// <param name="filename">Filename of image you want to download</param>
        /// <response code="200">Full resolution image</response>
        /// <response code="403">Logged user does not own this image</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("image/jpeg")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet, Route("{filename}/file")]
        public async Task<IActionResult> GetOriginalImage(string filename)
        {
            var login = GetAuthorizedUserLogin();

            var imageStream = await _imageManagerService.GetFileStream(filename, login);

            var filetype = Path.GetExtension(filename) switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                string extension => throw new UnsupportedMediaTypeException(extension),
                _ => throw new ApplicationException($"File without exception exists in base: {filename}")
            };

            return File(imageStream, filetype);
        }

        /// <summary>
        /// Refreshes lifetime of image
        /// </summary>
        /// <param name="filename">Filename of image you want to refresh</param>
        /// <response code="200">Lifetime refreshed</response>
        /// <response code="403">Logged user does not own this image</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MediaFileInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch, Route("{filename}/refresh")]
        public async Task<IActionResult> Refresh(string filename)
        {
            var login = GetAuthorizedUserLogin();
            var updatedImage = await _imageManagerService.Refresh(filename, login);

            return Ok(updatedImage);
        }

        /// <summary>
        /// Uploads new image
        /// </summary>
        /// <remarks>Allows only .jpg</remarks>
        /// <param name="file">Image in .jpg format to be uploaded</param>
        /// <response code="201">File has been created</response>
        /// <response code="415">Only .jpg extension is allowed</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MediaFileInfo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [HttpPost, Route("")]
        public async Task<IActionResult> Upload([Required] IFormFile file)
        {
            if (file is null)
                return StatusCode(415, "Image musn't be null");

            var extension = Path.GetExtension(file.FileName);
            if (!(extension == ".jpg" || extension == ".jpeg"))
                return StatusCode(415, "Image must be in .jpg format");


            var login = GetAuthorizedUserLogin();

            using var inputStream = file.OpenReadStream();
            var newImageInfo = await _imageManagerService.SaveFile(inputStream, file.FileName, login);

            return CreatedAtAction(nameof(GetOriginalImage), new { filename = newImageInfo.Filename }, newImageInfo);
        }

        /// <summary>
        /// Delete image from server
        /// </summary>
        /// <param name="filename">Filename of image you want to delete.</param>
        /// <response code="200">File has been deleted</response>
        /// <response code="403">Logged user does not own this image</response>
        /// <response code="404">File not found on server</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch, Route("{filename}/delete")]
        public async Task<IActionResult> Delete([Required] string filename)
        {
            var login = GetAuthorizedUserLogin();
            await _imageManagerService.DeleteFile(filename, login);

            return Ok();
        }

        private string GetAuthorizedUserLogin()
            => _jwtService.GetLoginFromToken(HttpContext.User.Identity as ClaimsIdentity);
    }
}
