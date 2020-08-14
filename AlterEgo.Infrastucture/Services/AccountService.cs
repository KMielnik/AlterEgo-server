using AlterEgo.Core.Domains;
using AlterEgo.Core.DTOs.Account;
using AlterEgo.Core.Interfaces;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Services
{
    public class AccountService : IAccountService
    {
        private readonly IJWTService _jwtService;
        private readonly ILogger<AccountService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IEncrypter _encrypter;

        public AccountService(
            IJWTService jwtService,
            ILogger<AccountService> logger,
            IUserRepository userRepository, 
            IEncrypter encrypter)
        {
            _jwtService = jwtService;
            _logger = logger;
            _userRepository = userRepository;
            _encrypter = encrypter;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            _logger.LogDebug("Starting authentication from data {@Request}", request);

            var user = await _userRepository.GetAsync(request.Login) ?? throw new AuthenticationFailedException();

            var hash = _encrypter.GetHash(request.Password, user.Salt);

            if (user.Password != hash)
                throw new AuthenticationFailedException();

            var response = new AuthenticationResponse
            {
                Login = user.Login,
                Nickname = user.Nickname,
                Email = user.Mail,
                JWToken = _jwtService.CreateToken(request.Login, "user")
            };

            _logger.LogDebug("Successfully authenticated {User}, returned response: {@Response}", user.Login, response);

            return response;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            _logger.LogDebug("Attempting registering user from request: {@Request}", request);

            var user = await _userRepository.GetAsync(request.Login);

            if (user != null)
                throw new UserAlreadyExistsException(request.Login);

            var salt = _encrypter.GetSalt(request.Password);
            var hash = _encrypter.GetHash(request.Password, salt);

            user = new User(
                request.Login,
                hash,
                salt,
                request.Nickname,
                request.Email);

            await _userRepository.AddAsync(user);

            _logger.LogDebug("User {UserName} registered.", user.Login);
        }
    }
}
