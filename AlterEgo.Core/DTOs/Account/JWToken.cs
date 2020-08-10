using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs.Account
{
    public class JWToken
    {
        public string Token { get; init; }
        public DateTime Expires { get; init; }
    }
}
