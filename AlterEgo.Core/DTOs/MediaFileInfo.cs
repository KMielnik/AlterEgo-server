using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs
{
    public class MediaFileInfo
    {
        /// <summary>
        /// Filename of active file
        /// </summary>
        /// <example>image.jpg</example>
        public string Filename { get; init; }

        /// <summary>
        /// Original filename
        /// </summary>
        /// <example>originalImage</example>
        public string OriginalFilename { get; init; }

        /// <summary>
        /// Login of user who owns this image
        /// </summary>
        /// <example>login123</example>
        public string UserLogin { get; init; }

        /// <summary>
        /// Time of planned deletion of resource
        /// </summary>
        public DateTime ExistsUntill { get; init; }

        /// <summary>
        /// Is the file available on the server.
        /// </summary>
        public bool IsAvailable { get; init; }

        /// <summary>
        /// Thumbnail in jpg format, included only when requested, otherwise null.
        /// </summary>
        public byte[] Thumbnail { get; init; }
    }
}
