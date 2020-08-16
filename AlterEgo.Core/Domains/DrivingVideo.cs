using AlterEgo.Core.Domains.Common;
using System;

namespace AlterEgo.Core.Domains
{
    public class DrivingVideo : MediaResource
    {
        public DrivingVideo(string filename, User owner, TimeSpan plannedLifetime, byte[] thumbnail) : base(filename, owner, plannedLifetime, thumbnail)
        {
        }

        public DrivingVideo()
        {

        }
    }
}
