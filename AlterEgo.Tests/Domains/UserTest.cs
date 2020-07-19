using AlterEgo.Core.Domains;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Tests.Domains
{
    [TestClass]
    public class UserTest
    {
        private static User _validUser;

        [ClassInitialize]
        public static void SetUp_ValidUser(TestContext context)
        {
            _validUser = new User(
                "login5",
                "pasdlkm#@@ddnjkasq1231",
                "pasdlkm#@@ddnjkasq1231",
                "Nickname9",
                "test.mail@gmail.com");
        }

        [TestMethod]
        public void CreateUser_ShouldSucceed_ValidCredentials()
        {
            var user = new User(
                _validUser.Login,
                _validUser.Password,
                _validUser.Salt,
                _validUser.Nickname,
                _validUser.Mail);

            Assert.IsNotNull(user);

            Assert.AreEqual(user.Login, _validUser.Login);
            Assert.AreEqual(user.Password, _validUser.Password);
            Assert.AreEqual(user.Salt, _validUser.Salt);
            Assert.AreEqual(user.Nickname, _validUser.Nickname);
            Assert.AreEqual(user.Mail, _validUser.Mail);
        }


        [DataRow("")]
        [DataRow("sho2")]
        [DataRow("sho2dlqwkedmqwldkqmwdlwkmdlwefjnewfksdnfksdfnwekfdslfmwelfnsdkfwemfkl")]
        [DataRow("!-@6<`.fsd,l;32")]
        [DataTestMethod]
        public void SetLogin_ThrowsException_LoginNotValid(string login)
        {
            Assert.ThrowsException<ArgumentException>(() => new User(
                login,
                _validUser.Password,
                _validUser.Salt,
                _validUser.Nickname,
                _validUser.Mail));
        }

        [DataRow(null,"")]
        [DataRow(null,null)]
        [DataRow("",null)]
        [DataTestMethod]
        public void SetPassword_ThrowsException_PasswordOrSaltNull(string password, string salt)
        {
            Assert.ThrowsException<ArgumentNullException>(() => new User(
                _validUser.Login,
                password,
                salt,
                _validUser.Nickname,
                _validUser.Mail));
        }
    }
}
