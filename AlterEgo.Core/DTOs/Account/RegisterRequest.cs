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
        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Login { get; init; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Password { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Required]
        public string Nickname { get; init; }
    }
}
