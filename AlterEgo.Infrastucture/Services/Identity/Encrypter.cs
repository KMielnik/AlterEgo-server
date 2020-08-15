using AlterEgo.Core.Interfaces.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Identity
{
    public class Encrypter : IEncrypter
    {
        public string GetHash(string password, string salt)
        {
            if (password is null)
                throw new ArgumentNullException(nameof(password), "Password cannot be null");
            if (salt is null)
                throw new ArgumentNullException(nameof(salt), "Salt cannot be null");

            var saltByteArray = new byte[salt.Length * sizeof(char)];
            Buffer.BlockCopy(salt.ToCharArray(), 0, saltByteArray, 0, saltByteArray.Length);

            var pbkdf2 = new Rfc2898DeriveBytes(password, saltByteArray, 10000);

            return Convert.ToBase64String(pbkdf2.GetBytes(50));
        }

        public string GetSalt(string password)
        {
            if (password is null)
                throw new ArgumentNullException(nameof(password), "Password cannot be null");

            var saltBytes = new byte[50];
            RandomNumberGenerator.Fill(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }
    }
}
