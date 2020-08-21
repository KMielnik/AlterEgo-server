using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AlterEgo.Core.DTOs;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlterEgo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IAnimationTaskService _taskService;
        private readonly IJWTService _jwtService;

        public TasksController(IAnimationTaskService taskService, IJWTService jwtService)
        {
            _taskService = taskService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Gets list of all active tasks of user
        /// </summary>
        /// <response code="200">Animation tasks list</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AnimationTaskDTO>), StatusCodes.Status200OK)]
        [HttpGet, Route("")]
        public async Task<IActionResult> GetAll()
        {
            var login = GetAuthorizedUserLogin();

            var allActiveTasks = await _taskService.GetAll(login).ToListAsync();

            return Ok(allActiveTasks);
        }

        /// <summary>
        /// Get specific task data
        /// </summary>
        /// <param name="id">Task data</param>
        /// <response code="200">Task info</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AnimationTaskDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetOne(string id)
        {
            var login = GetAuthorizedUserLogin();

            var task = await _taskService.GetSpecificTask(id, login);

            return Ok(task);
        }

        /// <summary>
        /// Adds new task to do
        /// </summary>
        /// <param name="request">Task data</param>
        /// <response code="201">New task created</response>
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AnimationTaskDTO), StatusCodes.Status201Created)]
        [HttpPost, Route("")]
        public async Task<IActionResult> Add(AnimationTaskRequest request)
        {
            var login = GetAuthorizedUserLogin();

            var newTask = await _taskService.AddNewTask(request, login);

            return CreatedAtAction(nameof(GetOne), new { id = newTask.Id.ToString() }, newTask);
        }

        private string GetAuthorizedUserLogin()
            => _jwtService.GetLoginFromToken(HttpContext.User.Identity as ClaimsIdentity);
    }
}
