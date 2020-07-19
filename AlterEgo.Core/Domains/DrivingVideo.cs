using System;

namespace AlterEgo.Core.Domains
{
    public class DrivingVideo : MediaResource
    {
        public DrivingVideo(string filename, User owner, TimeSpan plannedLifetime) : base(filename, owner, plannedLifetime)
        {
        }
    }
}
