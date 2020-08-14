using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs.Account
{
    public class RegisterRequest
    {
        /// <summary>
        /// Your login that you will use to log in.
        /// </summary>
        /// <example>login123</example>
        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Login { get; init; }

        /// <summary>
        /// Your password
        /// </summary>
        /// <example>pass123</example>
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Password { get; init; }

        /// <summary>
        /// Email address used for communication with you
        /// </summary>
        /// <example>email@gmail.com</example>
        [Required]
        [EmailAddress]
        public string Email { get; init; }

        /// <summary>
        /// Your nickname - used only for displaying
        /// </summary>
        /// <example>CoolGuy29</example>
        [Required]
        public string Nickname { get; init; }
    }
}
