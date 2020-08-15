using AlterEgo.Core.Domains.Common;
using AlterEgo.Core.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Interfaces.Animation
{
    public interface IGenericMediaManager<T> where T : MediaResource
    {
        Task<string> SaveFile(Stream inputStream, string originalFilename, string userLogin);
        Task<FileStream> GetFileStream(string filename, string userLogin);
        Task Refresh(string filename, string userLogin);
        IAsyncEnumerable<MediaFileInfo> GetAllActiveByUser(string userLogin);
    }
}
