using AlterEgo.Core.Domains.Common;
using System;

namespace AlterEgo.Core.Domains
{
    public class ResultVideo : MediaResource
    {
        public bool IsFinished { get; protected set; }

        private TimeSpan _plannedLifetime;
        public ResultVideo(string filename, string originalFilename, User owner, TimeSpan plannedLifetime, byte[] thumbnail) : base(filename, originalFilename, owner, TimeSpan.FromDays(365), thumbnail)
        {
            IsFinished = false;

            _plannedLifetime = plannedLifetime;
        }

        public void SetProcessingFinished(byte[] thumbnail)
        {
            Thumbnail = thumbnail;
            IsFinished = true;
            PlannedDeletion = CreatedAt.Add(_plannedLifetime);
        }
        public ResultVideo()
        {

        }
    }
}
