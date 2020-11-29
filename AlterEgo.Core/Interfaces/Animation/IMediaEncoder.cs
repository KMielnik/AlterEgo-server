using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Animation
{
    public interface IMediaEncoder
    {
        Task<byte[]> GetThumbnailAsync(string filepath);
        Task<string> ReencodeMedia(string filePath);
    }
}
