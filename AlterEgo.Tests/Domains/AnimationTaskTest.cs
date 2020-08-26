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
    public class AnimationTaskTest
    {
        private static User _user;
        private static Image _userImage;
        private static DrivingVideo _userDrivingVideo;
        private static ResultVideo _userResultVideo;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            _user = new User(
                "login5",
                "pasdlkm#@@ddnjkasq1231",
                "pasdlkm#@@ddnjkasq1231",
                "Nickname9",
                "test.mail@gmail.com");

            _userImage = new Image(
                "filename.jpg",
                _user,
                TimeSpan.Zero,
                null);

            _userDrivingVideo = new DrivingVideo(
                "filename.mp4",
                _user,
                TimeSpan.Zero,
                null);

            _userResultVideo = new ResultVideo(
                "resultFilename.mp4",
                _user,
                TimeSpan.Zero,
                null);
        }

        [TestMethod]
        public void CreateAnimationTask_ShouldSucceed_ValidCredentials()
        {
            var task = new AnimationTask(
                _user,
                _userDrivingVideo,
                _userImage,
                _userResultVideo);

            Assert.AreEqual(task.Status, AnimationTask.Statuses.New);
        }

        [TestMethod]
        public void SetStatusDone_ChangesStatus_IfCalled()
        {
            var task = new AnimationTask(
                _user,
                _userDrivingVideo,
                _userImage,
                _userResultVideo);

            task.SetStatusDone(null);

            Assert.AreEqual(task.Status, AnimationTask.Statuses.Done);
        }

        [TestMethod]
        public void CreateAnimationTask_ThrowsException_DiffrentMediaOwners()
        {
            var diffrentOwner = new User(
                "diffrentt1",
                "pass",
                "salt",
                "D1ffrent",
                "mail@wp.pl");

            var imageDiffrentOwner = new Image(
                "filename.png",
                diffrentOwner,
                TimeSpan.Zero,
                null);

            Assert.ThrowsException<ArgumentException>(() => new AnimationTask(
                _user,
                _userDrivingVideo,
                imageDiffrentOwner,
                _userResultVideo));
        }
    }
}
