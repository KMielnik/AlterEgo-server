using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs.Account
{
    public class JWToken
    {
        /// <summary>
        /// Your JWToken
        /// </summary>
        public string Token { get; init; }

        /// <summary>
        /// Expiration date for your token in UTC Datetime
        /// </summary>
        public DateTime Expires { get; init; }
    }
}
