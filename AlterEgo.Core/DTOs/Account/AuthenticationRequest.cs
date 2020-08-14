using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs.Account
{
    public class AuthenticationRequest
    {
        /// <summary>
        /// Your login
        /// </summary>
        /// <example>login123</example>
        [Required]
        public string Login { get; init; }

        /// <summary>
        /// Your password
        /// </summary>
        /// <example>pass123</example>
        [Required]
        public string Password { get; init; }
    }
}
