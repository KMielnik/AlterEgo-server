using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Exceptions
{
    public class AuthenticationFailedException : ApplicationException
    {
        public AuthenticationFailedException() : base("Invalid login or password")
        {
        }
    }
}
