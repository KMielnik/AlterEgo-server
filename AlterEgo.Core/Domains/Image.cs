using System;

namespace AlterEgo.Core.Domains
{
    public class Image : MediaResource
    {
        public Image(string filename, User owner, TimeSpan plannedLifetime) : base(filename, owner, plannedLifetime)
        {
        }
    }
}
