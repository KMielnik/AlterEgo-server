using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs.Account
{
    public class AuthenticationResponse
    {
        /// <summary>
        /// Your login
        /// </summary>
        /// <example>login123</example>
        public string Login { get; init; }

        /// <summary>
        /// Your email assigned to your account
        /// </summary>
        /// <example>login@gmail.com</example>
        public string Email { get; init; }

        /// <summary>
        /// Nickname you have choosen
        /// </summary>
        /// <example>CoolGuy29</example>
        public string Nickname { get; init; }

        /// <summary>
        /// Token used for authorization purposes
        /// </summary>
        public JWToken JWToken { get; init; }
    }
}
