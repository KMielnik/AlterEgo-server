using AlterEgo.Core.Domains.Common;
using System;

namespace AlterEgo.Core.Domains
{
    public class Image : MediaResource
    {
        public Image(string filename, string originalFilename, User owner, TimeSpan plannedLifetime, byte[] thumbnail) : base(filename, originalFilename, owner, plannedLifetime, thumbnail)
        {
        }

        public Image()
        {

        }
    }
}
