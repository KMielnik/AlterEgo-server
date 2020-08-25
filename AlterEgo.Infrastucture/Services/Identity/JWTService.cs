using AlterEgo.Core.DTOs.Account;
using AlterEgo.Core.Interfaces.Identity;
using AlterEgo.Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Identity
{
    public class JWTService : IJWTService
    {
        private readonly JWTSettings _jwtSettings;

        public JWTService(IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public JWToken CreateToken(string login, string role)
        {
            var now = DateTime.UtcNow;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, login),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var expires = now.AddMinutes(_jwtSettings.ExpiresAfter);

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: signingCredentials,
                notBefore: now,
                expires: expires);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JWToken
            {
                Token = token,
                Expires = expires
            };
        }

        public string GetLoginFromToken(ClaimsIdentity identity)
        {
            if(identity is null)
                throw new UnauthorizedAccessException("No authorized user avaiable for getting login.");

            var loginClaim = identity.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti) ?? throw new ApplicationException("Jti claim was not found in token.");

            return loginClaim.Value;
        }
    }
}
