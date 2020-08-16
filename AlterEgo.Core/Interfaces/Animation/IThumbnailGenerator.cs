using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Animation
{
    public interface IThumbnailGenerator
    {
        Task<byte[]> GetThumbnailAsync(string filepath);
    }
}
