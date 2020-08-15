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
    public class ResultVideoManagerService : GenericMediaManager<ResultVideo>, IResultVideoManagerService
    {
        public ResultVideoManagerService(
            IOptions<FilesLocationSettings> filesLocationSettings,
            IResultVideoRepository resultVideoRepository,
            IUserRepository userRepository,
            ILogger<ResultVideoManagerService> logger)
            : base(resultVideoRepository,
                  userRepository,
                  filesLocationSettings.Value.OutputDirectory,
                  logger)
        { }
    }
}
