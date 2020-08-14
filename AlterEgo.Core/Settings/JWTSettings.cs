using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Settings
{
    public class JWTSettings
    {
        public string SecretKey { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int ExpiresAfter { get; init; }
    }
}
