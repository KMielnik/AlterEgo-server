using System;
using System.Text.RegularExpressions;

namespace AlterEgo.Core.Domains
{
    public class User
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Salt { get; private set; }
        public string Nickname { get; private set; }
        public string Mail { get; private set; }

        public User(string login,
            string password,
            string salt,
            string nickname,
            string mail)
        {
            SetLogin(login);
            SetPassword(password, salt);
            SetNickname(nickname);
            SetMail(mail);
        }

        private void SetLogin(string login)
            => Login = login switch
            {
                null => throw new ArgumentNullException(nameof(login)),
                string s when !Regex.IsMatch(s, "^[a-z0-9_-]+$") => throw new ArgumentException("Illegal characters.",
                    nameof(login)),
                { Length: < 5 or > 30 } => throw new ArgumentException("Length must be between 5 and 30 characters.",
                    nameof(login)),
                _ => login,
            };

        private void SetPassword(string password, string salt)
            => (Password, Salt) = (password, salt) switch
            {
                (null, _) => throw new ArgumentNullException(nameof(password)),
                (_, null) => throw new ArgumentNullException(nameof(salt)),
                _ => (password, salt),
            };

        private void SetNickname(string nickname)
            => Nickname = nickname switch
            {
                null => throw new ArgumentNullException(nameof(nickname)),
                string s when !Regex.IsMatch(s, @"^[A-Za-z0-9_-]+$") => throw new ArgumentException(
                    "Illegal characters.", nameof(nickname)),
                { Length: <= 0 } => throw new ArgumentException("Length must not be empty", nameof(nickname)),
                _ => nickname,
            };

        private void SetMail(string mail)
            => Mail = mail switch
            {
                { Length: <= 0 } => null,
                null => null,
                string s when !Regex.IsMatch(s, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") =>
                throw new ArgumentException("Illegal mail.", nameof(mail)),
                _ => mail.ToLowerInvariant(),
            };

        public override string ToString()
            => $"{{User: {{Login: {Login}}}, {{Mail: {Mail}}}}}";

        public override bool Equals(object obj)
            => obj is User u && u.Login == Login;

        public override int GetHashCode()
            => Login.GetHashCode();
    }
}
