using AlterEgo.Core.Domains;
using AlterEgo.Core.DTOs;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Animation
{
    public class ImageManagerService : GenericMediaManager<Image>, IImageManagerService
    {
        public ImageManagerService(
            IOptions<FilesLocationSettings> filesLocationSettings,
            IImageRepository imageRepository,
            IUserRepository userRepository,
            ILogger<ImageManagerService> logger,
            IThumbnailGenerator thumbnailGenerator)
            : base(imageRepository,
                  userRepository,
                  filesLocationSettings.Value.ImagesDirectory,
                  logger,
                  thumbnailGenerator)
        { }
    }
}
