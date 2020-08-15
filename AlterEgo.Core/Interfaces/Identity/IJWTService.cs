using AlterEgo.Core.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Identity
{
    public interface IJWTService
    {
        JWToken CreateToken(string login, string role);
    }
}
