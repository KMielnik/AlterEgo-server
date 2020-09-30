using AlterEgo.Core.Domains;
using AlterEgo.Core.DTOs;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Animation
{
    public class AnimationTaskService : IAnimationTaskService
    {
        private readonly IAnimationTaskRepository _taskRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IDrivingVideoRepository _drivingVideoRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AnimationTaskService> _logger;

        public AnimationTaskService(
            IAnimationTaskRepository taskRepository,
            ILogger<AnimationTaskService> logger,
            IImageRepository imageRepository,
            IDrivingVideoRepository drivingVideoRepository,
            IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _logger = logger;
            _imageRepository = imageRepository;
            _drivingVideoRepository = drivingVideoRepository;
            _userRepository = userRepository;
        }

        public async Task<AnimationTaskDTO> AddNewTask(AnimationTaskRequest request, string userLogin)
        {
            _logger.LogDebug("Adding requested {@Task} to database", request);

            var user = await _userRepository.GetAsync(userLogin);

            var image = await _imageRepository.GetAsync(request.SourceImage) ?? throw new OwnerMismatchException("User does not own image with that filename");
            var drivingVideo = await _drivingVideoRepository.GetAsync(request.SourceVideo) ?? throw new OwnerMismatchException("User does not own video with that filename");
            var resultVideo = new ResultVideo(
                Guid.NewGuid() + ".mp4",
                user,
                TimeSpan.FromHours(6),
                null);

            if (image.Owner.Login != user.Login)
                throw new OwnerMismatchException($"{userLogin} does not own requested image.");

            if (drivingVideo.Owner.Login != user.Login)
                throw new OwnerMismatchException($"{userLogin} does not own requested video.");

            var task = new AnimationTask(user, drivingVideo, image, resultVideo, request.RetainAudio, request.ImagePadding);

            _logger.LogDebug("Adding {@Task} to database.", task);

            task = await _taskRepository.AddAsync(task);

            _logger.LogDebug("Added {@Task} to database.", task);

            return ConvertToDTO(task);
        }

        public IAsyncEnumerable<AnimationTaskDTO> GetAll(string userLogin)
        => _taskRepository.GetAllAsync()
                .Where(x => x.Owner.Login == userLogin)
                .Select(x => ConvertToDTO(x));


        public async Task<AnimationTaskDTO> GetSpecificTask(string id, string userLogin)
        {
            var task = await _taskRepository.GetAsync(Guid.Parse(id));

            _logger.LogDebug("Searched for task {ID}, found {@Task}", id, task);

            if (task is null)
                throw new FileNotFoundException($"Task with id {id} does not exist in database");
            if (task.Owner.Login != userLogin)
                throw new OwnerMismatchException($"User {userLogin} does not own the task {id}.");

            return ConvertToDTO(task);
        }

        private AnimationTaskDTO ConvertToDTO(AnimationTask task)
        => new AnimationTaskDTO
        {
            Id = task.Id,
            Owner = task.Owner.Login,
            SourceVideo = task.SourceVideo.Filename,
            SourceImage = task.SourceImage.Filename,
            ResultAnimation = task.ResultAnimation.Filename,
            RetainAudio = task.RetainAudio,
            ImagePadding = task.ImagePadding,
            CreatedAt = task.CreatedAt,
            Status = task.Status
        };
    }
}
