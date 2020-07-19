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
    public class DrivingVideoTest
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
        public void CreateDrivingVideo_ShouldSucceed_ValidCredentials()
        {
            var filename = "filename.mp4";
            var plannedTimespan = TimeSpan.FromMinutes(5);

            var video = new DrivingVideo(
                filename,
                _validUser,
                plannedTimespan);

            Assert.IsNotNull(video);

            Assert.AreEqual(video.Filename, filename);
            Assert.AreEqual(video.PlannedDeletion, video.CreatedAt.Add(plannedTimespan));
            Assert.AreEqual(video.Owner, _validUser);
        }

        [DataRow("")]
        [DataRow("!-@6<`.fsd,l;32")]
        [DataTestMethod]
        public void SetFilename_ThrowsException_FilenameNotValid(string filename)
        {
            Assert.ThrowsException<ArgumentException>(()
                => new DrivingVideo(
                    filename,
                    _validUser,
                    TimeSpan.Zero));
        }
    }
}
