﻿using AlterEgo.Core.Domains;
using AlterEgo.Core.Domains.Common;
using AlterEgo.Core.DTOs;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastructure.Exceptions;
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

        private readonly string _filesLocationPath;
        private readonly ILogger<GenericMediaManager<T>> _logger;

        public GenericMediaManager(
            IGenericMediaRepository<T> mediaRepository, 
            IUserRepository userRepository, 
            string filesLocationPath, 
            ILogger<GenericMediaManager<T>> logger)
        {
            _mediaRepository = mediaRepository;
            _userRepository = userRepository;
            _filesLocationPath = filesLocationPath;
            _logger = logger;
        }

        public async IAsyncEnumerable<MediaFileInfo> GetAllActiveByUser(string userLogin)
        {
            _logger.LogDebug("Getting all active {MediaType} for {Login}", typeof(T), userLogin);

            var medias = _mediaRepository.GetAllAsync()
                .Where(x => x.Owner.Login == userLogin)
                .Where(x => x.ActualDeletion is null);

            await foreach (var media in medias)
            {
                _logger.LogDebug("Active {MediaType} found - {@Media}", typeof(T), media);

                yield return new MediaFileInfo
                {
                    Filename = media.Filename,
                    UserLogin = media.Owner.Login,
                    ExistsUntill = media.PlannedDeletion
                };
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

        public async Task Refresh(string filename, string userLogin)
        {
            var file = await GetFile(filename, userLogin);

            file.RefreshPlannedDeletion();

            await _mediaRepository.UpdateAsync(file);

            _logger.LogDebug("Refreshed planned deletion time of {@File}", file);
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

        public async Task<string> SaveFile(Stream inputStream, string originalFilename, string userLogin)
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

            var newMedia = (T)Convert.ChangeType(typeof(T) switch
            {
                Type t when t == typeof(Image) => new Image(newFilename, user, TimeSpan.FromHours(6)),
                Type t when t == typeof(DrivingVideo) => new DrivingVideo(newFilename, user, TimeSpan.FromHours(6)),
                Type t when t == typeof(ResultVideo) => new ResultVideo(newFilename, user, TimeSpan.FromHours(6)),
                _ => throw new ApplicationException($"Unkonwn type of Media, {typeof(Image)} is not known handled")
            }, typeof(T));

            newMedia = await _mediaRepository.AddAsync(newMedia);

            _logger.LogDebug("Saved entry {@MediaEntry} to database", newMedia);
            return newMedia.Filename;
        }
    }
}