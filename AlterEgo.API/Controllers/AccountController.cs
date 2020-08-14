using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AlterEgo.Core.DTOs.Account;
using AlterEgo.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlterEgo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="request">Your authentication data</param>
        /// <response code="200">You have been logged in correctly</response>
        /// <response code="400">Could not find user with this login/password combination</response>
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, Route("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
            => Ok(await _accountService.AuthenticateAsync(request));


        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="request">Request data, with which new user will be created</param>
        /// <response code="200">User has been created</response>
        /// <response code="409">User with this login already exists</response>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost, Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            await _accountService.RegisterAsync(request);
            return Ok();
        }
    }
}
