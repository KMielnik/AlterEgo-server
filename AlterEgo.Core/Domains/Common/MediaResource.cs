using System;
using System.Text.RegularExpressions;

namespace AlterEgo.Core.Domains.Common
{
    public abstract class MediaResource
    {
        public string Filename { get; protected set; }
        public string OriginalFilename { get; protected set; }
        public User Owner { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime PlannedDeletion { get; protected set; }
        public DateTime? ActualDeletion { get; protected set; }

        public byte[] Thumbnail { get; protected set; }

        public TimeSpan PlannedLifetime { get; protected set; }

        public MediaResource(string filename, string originalFilename, User owner, TimeSpan plannedLifetime, byte[] thumbnail)
        {
            SetFilename(filename);
            SetOriginalFilename(originalFilename);
            SetOwner(owner);

            PlannedLifetime = plannedLifetime;

            CreatedAt = DateTime.Now;
            SetPlannedDeletion(plannedLifetime);
            ActualDeletion = null;

            SetThumbnail(thumbnail);
        }

        private void SetThumbnail(byte[] thumbnail)
            => Thumbnail = thumbnail switch
            {
                { Length: 0} => throw new ArgumentException("Cannot be empty", nameof(thumbnail)),
                _ => thumbnail
            };

        public void SetActualDeletionTime(DateTime actualDeletion)
            => ActualDeletion = actualDeletion;

        public void RefreshPlannedDeletion()
            => PlannedDeletion = DateTime.Now + PlannedLifetime;

        private void SetFilename(string filename)
            => Filename = filename switch
            {
                null => throw new ArgumentNullException(nameof(filename)),
                { Length: <= 0 } => throw new ArgumentException("Cannot be empty", nameof(filename)),
                string s when !Regex.IsMatch(s, "^[A-Za-z0-9_.-]+$") => throw new ArgumentException("Illegal characters.",
                    nameof(filename)),
                _ => filename,
            };

        private void SetOriginalFilename(string originalFilename)
            => OriginalFilename = originalFilename switch
            {
                null => throw new ArgumentNullException(nameof(originalFilename)),
                { Length: <= 0 } => throw new ArgumentException("Cannot be empty", nameof(originalFilename)),
                string s when !Regex.IsMatch(s, "^[A-Za-z0-9_-]+$") => throw new ArgumentException("Illegal characters.",
                    nameof(originalFilename)),
                _ => originalFilename,
            };

        private void SetOwner(User owner)
            => Owner = owner switch
            {
                null => throw new ArgumentNullException(nameof(owner)),
                _ => owner,
            };

        private void SetPlannedDeletion(TimeSpan plannedLifetime)
            => PlannedDeletion = CreatedAt.Add(plannedLifetime);

        public MediaResource()
        {

        }
    }
}
