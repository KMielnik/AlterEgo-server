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
        [Required]
        public string Login { get; init; }

        [Required]
        public string Password { get; init; }
    }
}
