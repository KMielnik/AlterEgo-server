using AlterEgo.Core.Domains.Common;
using System;

namespace AlterEgo.Core.Domains
{
    public class ResultVideo : MediaResource
    {
        public bool IsFinished { get; protected set; }

        private TimeSpan _plannedLifetime;
        public ResultVideo(string filename, User owner, TimeSpan plannedLifetime) : base(filename, owner, TimeSpan.FromDays(365))
        {
            IsFinished = false;

            _plannedLifetime = plannedLifetime;
        }

        public void SetProcessingFinished()
        {
            IsFinished = true;
            PlannedDeletion = CreatedAt.Add(_plannedLifetime);
        }
    }
}
