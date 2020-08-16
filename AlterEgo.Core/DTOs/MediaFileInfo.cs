using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.DTOs
{
    public class MediaFileInfo
    {
        public string Filename { get; init; }
        public string UserLogin { get; init; }
        public DateTime ExistsUntill { get; init; }
        public byte[] Thumbnail { get; init; }
    }
}
