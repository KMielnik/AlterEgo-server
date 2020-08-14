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

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost, Route("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
            => Ok(await _accountService.AuthenticateAsync(request));

        [AllowAnonymous]
        [HttpPost, Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            await _accountService.RegisterAsync(request);
            return Ok();
        }
    }
}
