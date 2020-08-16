using AlterEgo.Core.Domains.Common;
using System;

namespace AlterEgo.Core.Domains
{
    public class Image : MediaResource
    {
        public Image(string filename, User owner, TimeSpan plannedLifetime, byte[] thumbnail) : base(filename, owner, plannedLifetime, thumbnail)
        {
        }

        public Image()
        {

        }
    }
}
