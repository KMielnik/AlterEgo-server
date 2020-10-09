using AlterEgo.Core.Domains;
using AlterEgo.Core.Domains.Common;
using AlterEgo.Core.DTOs;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastructure.Exceptions;
using FFmpeg.NET;
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
    public class GenericMediaManager<T> : IGenericMediaManager<T> where T : MediaResource
    {
        private readonly IGenericMediaRepository<T> _mediaRepository;
        private readonly IUserRepository _userRepository;
        private readonly IThumbnailGenerator _thumbnailGenerator;

        private readonly string _filesLocationPath;
        private readonly ILogger<GenericMediaManager<T>> _logger;

        public GenericMediaManager(
            IGenericMediaRepository<T> mediaRepository,
            IUserRepository userRepository,
            string filesLocationPath,
            ILogger<GenericMediaManager<T>> logger,
            IThumbnailGenerator thumbnailGenerator)
        {
            _mediaRepository = mediaRepository;
            _userRepository = userRepository;
            _filesLocationPath = filesLocationPath;
            _logger = logger;
            _thumbnailGenerator = thumbnailGenerator;
        }

        public async IAsyncEnumerable<MediaFileInfo> GetAllActiveByUser(string userLogin, bool includeThumbnails)
        {
            _logger.LogDebug("Getting all active {MediaType} for {Login}", typeof(T), userLogin);

            var activeMediaFiles = GetAllByUser(userLogin, includeThumbnails)
                .Where(x => x.IsAvailable);

            await foreach (var activeMediaFile in activeMediaFiles)
            {
                _logger.LogTrace("Active {MediaType} found - {@Media}", typeof(T), activeMediaFile);

                yield return activeMediaFile;
            }

            _logger.LogDebug("Finished getting all active {MediaType} for {Login}", typeof(T), userLogin);
        }

        public async Task<FileStream> GetFileStream(string filename, string userLogin)
        {
            var file = await GetFile(filename, userLogin);

            var filePath = Path.Combine(_filesLocationPath, file.Filename);

            _logger.LogDebug("File {FilePath} is being opened for user.", filePath);

            var fileStream = File.OpenRead(filePath);

            return fileStream;
        }

        public async Task<MediaFileInfo> Refresh(string filename, string userLogin)
        {
            var file = await GetFile(filename, userLogin);

            file.RefreshPlannedDeletion();

            await _mediaRepository.UpdateAsync(file);

            _logger.LogDebug("Refreshed planned deletion time of {@File}", file);

            return ConvertToMediaFileInfo(file, false);
        }

        private async Task<T> GetFile(string filename, string userLogin)
        {
            _logger.LogDebug("Trying to obtain {MediaType} {Filename} for {UserLogin}", typeof(T), filename, userLogin);

            var user = await _userRepository.GetAsync(userLogin) ?? throw new UnauthorizedAccessException($"Could not find user with that login [{userLogin}]");

            var file = await _mediaRepository.GetAsync(filename) ?? throw new FileNotFoundException($"Couldn't find requested file {filename}");

            if (file.Owner.Login != user.Login)
            {
                _logger.LogError("{@User} does not own the {@File}}", user, file);

                throw new OwnerMismatchException($"User {user.Login} does not own the file {file.Filename}");
            }

            return file;
        }

        private MediaFileInfo ConvertToMediaFileInfo(T item, bool includeThumbnails)
            => new MediaFileInfo
            {
                Filename = item.Filename,
                OriginalFilename = item.OriginalFilename,
                UserLogin = item.Owner.Login,
                ExistsUntill = item.PlannedDeletion,
                IsAvailable = item.ActualDeletion is null,
                Thumbnail = includeThumbnails ? item.Thumbnail : null
            };

        public async Task<MediaFileInfo> SaveFile(Stream inputStream, string originalFilename, string userLogin)
        {
            _logger.LogDebug("Saving file received from {UserLogin}", userLogin);

            var user = await _userRepository.GetAsync(userLogin) ?? throw new UnauthorizedAccessException($"Could not find user with that login [{userLogin}]");

            _logger.LogDebug("Found correct user");

            var newFilename = Guid.NewGuid() + Path.GetExtension(originalFilename);
            var newPath = Path.Combine(_filesLocationPath, newFilename);

            _logger.LogDebug("Planned path of the new {MediaType}: \"{Path}\"", typeof(T), newPath);

            using (var outputStream = File.Create(newPath))
            {
                await inputStream.CopyToAsync(outputStream);
            }

            _logger.LogDebug("Saved {MediaType} {Filename}", typeof(T), newFilename);

            _logger.LogDebug("Generationg thumbnail.");

            var thumbnail = await _thumbnailGenerator.GetThumbnailAsync(newPath);

            _logger.LogDebug("Thumbnail generated.");

            var newMedia = (T)Convert.ChangeType(typeof(T) switch
            {
                Type t when t == typeof(Image) => new Image(newFilename, Path.GetFileNameWithoutExtension(originalFilename), user, TimeSpan.FromDays(2), thumbnail),
                Type t when t == typeof(DrivingVideo) => new DrivingVideo(newFilename, Path.GetFileNameWithoutExtension(originalFilename), user, TimeSpan.FromDays(2), thumbnail),
                Type t when t == typeof(ResultVideo) => new ResultVideo(newFilename, Path.GetFileNameWithoutExtension(originalFilename), user, TimeSpan.FromDays(2), thumbnail),
                _ => throw new ApplicationException($"Unkonwn type of Media, {typeof(T)} is not known.")
            }, typeof(T));

            newMedia = await _mediaRepository.AddAsync(newMedia);

            _logger.LogDebug("Saved entry {@MediaEntry} to database", newMedia);

            return ConvertToMediaFileInfo(newMedia, false);
        }

        public async Task DeleteFile(string filename, string userLogin)
        {
            _logger.LogDebug("Deleting {Filename} received from {UserLogin}", filename, userLogin);

            var user = await _userRepository.GetAsync(userLogin) ?? throw new UnauthorizedAccessException($"Could not find user with that login [{userLogin}]");
            var expiredFile = await _mediaRepository.GetAsync(filename) ?? throw new FileNotFoundException($"Couldn't find requested file {filename}");

            var filepath = Path.Combine(_filesLocationPath, expiredFile.Filename);

            File.Delete(filepath);

            expiredFile.SetActualDeletionTime(DateTime.UtcNow);

            await _mediaRepository.UpdateAsync(expiredFile);

            _logger.LogDebug("File of type {TypeName} - {@ExpiredItem} deleted successfully.", typeof(T).Name, expiredFile);
        }

        public async IAsyncEnumerable<MediaFileInfo> GetAllByUser(string userLogin, bool includeThumbnails)
        {
            _logger.LogDebug("Getting all {MediaType} for {Login}", typeof(T), userLogin);

            var medias = _mediaRepository.GetAllAsync()
                .Where(x => x.Owner.Login == userLogin)
                .OrderByDescending(x => x.PlannedDeletion);

            await foreach (var media in medias)
            {
                _logger.LogTrace("{MediaType} found - {@Media}", typeof(T), media);

                yield return ConvertToMediaFileInfo(media, includeThumbnails);
            }

            _logger.LogDebug("Finished getting all {MediaType} for {Login}", typeof(T), userLogin);
        }
    }
}
