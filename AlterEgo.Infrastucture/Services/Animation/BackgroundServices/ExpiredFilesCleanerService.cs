using AlterEgo.Core.Domains;
using AlterEgo.Core.Domains.Common;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Animation.BackgroundServices
{
    public class ExpiredFilesCleanerService : BackgroundService
    {
        private readonly ILogger<ExpiredFilesCleanerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly FilesLocationSettings _filesLocationSettings;

        public ExpiredFilesCleanerService(
            ILogger<ExpiredFilesCleanerService> logger,
            IServiceScopeFactory scopeFactory,
            IHostApplicationLifetime appLifetime, 
            IOptions<FilesLocationSettings> filesLocationSettings) : base(logger, appLifetime)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _filesLocationSettings = filesLocationSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("{ServiceName} started", nameof(ExpiredFilesCleanerService));

            cancellationToken.Register(() => _logger.LogDebug("Animation tasks service is stopping"));

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _imageRepository = scope.ServiceProvider.GetRequiredService<IImageRepository>();
                    var _drivingVideoRepository = scope.ServiceProvider.GetRequiredService<IDrivingVideoRepository>();
                    var _resultVideoRepository = scope.ServiceProvider.GetRequiredService<IResultVideoRepository>();

                    await CleanExpiredFiles(_imageRepository);
                    await CleanExpiredFiles(_drivingVideoRepository);
                    await CleanExpiredFiles(_resultVideoRepository);
                }

                await Task.Delay(5000, cancellationToken);
            }

            _logger.LogDebug("{ServiceName} is stopping", nameof(ExpiredFilesCleanerService));
        }

        private async Task CleanExpiredFiles<T>(IGenericMediaRepository<T> repository) where T: MediaResource
        {
            var expiredItems = await repository
                .GetAllAsync()
                .Where(x => x.ActualDeletion is null)
                .Where(x => x.PlannedDeletion < DateTime.UtcNow)
                .ToListAsync();

            var location = typeof(T) switch
            {
                Type t when t == typeof(Image) => _filesLocationSettings.ImagesDirectory,
                Type t when t == typeof(DrivingVideo) => _filesLocationSettings.VideosDirectory,
                Type t when t == typeof(ResultVideo) => _filesLocationSettings.OutputDirectory,
                _ => throw new ApplicationException($"Unkown type of Media, {typeof(T)} is not known")
            };

            foreach(var expiredItem in expiredItems)
            {
                _logger.LogDebug("File of type {TypeName} - {@ExpiredItem} has expired - deleting.", typeof(T).Name, expiredItem);

                var fileLocation = Path.Combine(location, expiredItem.Filename);

                File.Delete(fileLocation);

                expiredItem.SetActualDeletionTime(DateTime.UtcNow);

                await repository.UpdateAsync(expiredItem);

                _logger.LogDebug("File of type {TypeName} - {@ExpiredItem} deleted successfully.", typeof(T).Name, expiredItem);
            }
        }
    }
}
