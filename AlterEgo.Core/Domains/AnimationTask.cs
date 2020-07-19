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
        }

        public User Owner { get; protected set; }
        public DrivingVideo SourceVideo { get; protected set; }
        public Image SourceImage { get; protected set; }
        public ResultVideo ResultAnimation { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public Statuses Status { get; protected set; }

        public AnimationTask(User owner, DrivingVideo sourceVideo, Image sourceImage, ResultVideo resultAnimation)
        {
            SetOwner(owner);
            SetSourceVideo(sourceVideo);
            SetSourceImage(sourceImage);
            SetResultAnimation(resultAnimation);

            CreatedAt = DateTime.Now;

            Status = Statuses.New;
        }

        public void SetStatusProcessing()
            => Status = Statuses.Processing;

        public void SetStatusDone()
        {
            ResultAnimation.SetProcessingFinished();
            Status = Statuses.Done;
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
    }
}
