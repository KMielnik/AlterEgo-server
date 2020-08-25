using System;

namespace AlterEgo.Core.Domains
{
    public class AnimationTask
    {
        public enum Statuses
        {
            New,
            Processing,
            Done,
            Notified,
            Failed,
        }

        public Guid Id { get; protected set; }
        public User Owner { get; protected set; }
        public DrivingVideo SourceVideo { get; protected set; }
        public Image SourceImage { get; protected set; }
        public ResultVideo ResultAnimation { get; protected set; }
        public bool RetainAudio { get; protected set; }
        public float ImagePadding { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public Statuses Status { get; protected set; }

        public AnimationTask(User owner, DrivingVideo sourceVideo, Image sourceImage, ResultVideo resultAnimation, bool retainAudio = true, float imagePadding = 0.2f)
        {
            Id = Guid.NewGuid();

            SetOwner(owner);
            SetSourceVideo(sourceVideo);
            SetSourceImage(sourceImage);
            SetResultAnimation(resultAnimation);

            RetainAudio = retainAudio;
            SetImagePadding(imagePadding);

            CreatedAt = DateTime.Now;

            Status = Statuses.New;
        }

        public void SetStatusProcessing()
            => Status = Statuses.Processing;

        public void SetStatusDone(byte[] thumbnail)
        {
            ResultAnimation.SetProcessingFinished(thumbnail);
            Status = Statuses.Done;
        }

        public void SetStatusNotified()
            => Status = Statuses.Notified;

        public void SetStatusFailed()
            => Status = Statuses.Failed;

        private void SetImagePadding(float imagePadding)
        {
            if (imagePadding < 0 || imagePadding > 1)
                throw new ArgumentException($"Image padding should be between 0.0 and 1.0 (was {imagePadding})", nameof(imagePadding));
            ImagePadding = ImagePadding;
        }

        private void SetOwner(User owner)
            => Owner = owner switch
            {
                null => throw new ArgumentNullException(nameof(owner)),
                _ => owner,
            };

        private void SetSourceVideo(DrivingVideo sourceVideo)
            => SourceVideo = sourceVideo switch
            {
                null => throw new ArgumentNullException(nameof(sourceVideo)),
                DrivingVideo video when !video.Owner.Equals(Owner) => throw new ArgumentException("Assigned owner does not own this media resource", nameof(sourceVideo)),
                _ => sourceVideo,
            };

        private void SetSourceImage(Image sourceImage)
            => SourceImage = sourceImage switch
            {
                null => throw new ArgumentNullException(nameof(sourceImage)),
                Image image when !image.Owner.Equals(Owner) => throw new ArgumentException("Assigned owner does not own this media resource", nameof(sourceImage)),
                _ => sourceImage,
            };

        private void SetResultAnimation(ResultVideo resultAnimation)
            => ResultAnimation = resultAnimation switch
            {
                null => throw new ArgumentNullException(nameof(resultAnimation)),
                ResultVideo video when !video.Owner.Equals(Owner) => throw new ArgumentException("Assigned owner does not own this media resource", nameof(resultAnimation)),
                _ => resultAnimation,
            };

        public AnimationTask()
        {

        }
    }
}
