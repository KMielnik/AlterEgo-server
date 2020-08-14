using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces
{
    public interface IEncrypter
    {
        string GetSalt(string password);
        string GetHash(string password, string salt);
    }
}
