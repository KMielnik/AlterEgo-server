using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs
{
    public class AnimationTaskRequest
    {
        /// <summary>
        /// Filename of source video
        /// </summary>
        /// <example>video.mp4</example>
        public string SourceVideo { get; init; }

        /// <summary>
        /// Filename of source image
        /// </summary>
        /// <example>video.jpg</example>
        public string SourceImage { get; init; }

        /// <summary>
        /// Value indicating if audio retention was requested.
        /// </summary>
        /// <example>true</example>
        public bool RetainAudio { get; init; }

        /// <summary>
        /// Amount of image padding (scale 0.0-1.0) in animation requested.
        /// </summary>
        /// <example>0.2</example>
        public float ImagePadding { get; init; }
    }
}
