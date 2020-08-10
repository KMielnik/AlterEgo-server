using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs.Account
{
    public class AuthenticationResponse
    {
        public string Login { get; init; }
        public string Email { get; init; }
        public string Nickname { get; init; }
        public JWToken JWToken { get; init; }
    }
}
