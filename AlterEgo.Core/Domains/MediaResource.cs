using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlterEgo.Core.Domains
{
    public abstract class MediaResource
    {
        public string Filename { get; private set; }
        public User Owner { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime PlannedDeletion { get; private set; }
        public DateTime? ActualDeletion { get; private set; }

        public MediaResource(string filename, User owner, TimeSpan plannedLifetime)
        {
            SetFilename(filename);
            SetOwner(owner);

            CreatedAt = DateTime.Now;
            SetPlannedDeletion(plannedLifetime);
            ActualDeletion = null;
        }

        private void SetFilename(string filename)
            => Filename = filename switch
            {
                null => throw new ArgumentNullException(nameof(filename)),
                { Length: <= 0 } => throw new ArgumentException("Cannot be empty", nameof(filename)),
                string s when !Regex.IsMatch(s, "^[A-Za-z0-9_.-]+$") => throw new ArgumentException("Illegal characters.",
                    nameof(filename)),
                _ => filename,
            };

        private void SetOwner(User owner)
            => Owner = owner switch
            {
                null => throw new ArgumentNullException(nameof(owner)),
                _ => owner,
            };

        private void SetPlannedDeletion(TimeSpan plannedLifetime)
            => PlannedDeletion = CreatedAt.Add(plannedLifetime);
    }
}
