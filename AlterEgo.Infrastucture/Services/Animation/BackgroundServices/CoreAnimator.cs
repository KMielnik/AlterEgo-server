using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Animation.BackgroundServices
{
    public class CoreAnimator : IAnimator
    {
        protected readonly CoreAnimatorSettings _animatorSettings;
        protected readonly FilesLocationSettings _filesLocationSettings;
        private readonly ILogger<CoreAnimator> _logger;
        private readonly IThumbnailGenerator _thumbnailGenerator;

        public CoreAnimator(
            IOptions<CoreAnimatorSettings> animatorSettings,
            IOptions<FilesLocationSettings> filesLocationSettings,
            ILogger<CoreAnimator> logger, 
            IThumbnailGenerator thumbnailGenerator)
        {
            _animatorSettings = animatorSettings.Value;
            _filesLocationSettings = filesLocationSettings.Value;

            _logger = logger ?? NullLogger<CoreAnimator>.Instance;

            _logger.LogInformation("CoreAnimator initialized");
            _logger.LogInformation("CoreAnimators settings - {@Settings}", _animatorSettings);
            _thumbnailGenerator = thumbnailGenerator;
        }

        public async Task Animate(AnimationTask task)
        {
            _logger.LogInformation("Processing {ResultFilename} started", task.ResultAnimation.Filename);
            _logger.LogDebug("Task being processed by CoreAnimator - {@Task}", task);

            task.SetStatusProcessing();

            var builder = GetPreconfiguredBuilder();

            builder.WithDrivingVideo(task.SourceVideo)
                .AddResultAnimation(task.SourceImage, task.ResultAnimation);

            if (task.RetainAudio)
                builder.WithAudio();

            builder.WithCustomImagePadding(task.ImagePadding);

            var command = builder.Build();

            _logger.LogDebug("Animator command - {Command}", command);

            await foreach (var ev in RunAnimationProcessAsync(command))
            {
                _logger.LogInformation("Animator returned event - {OutputEvent}", ev);

                switch (ev.EventType.Name)
                {
                    case EventType.VIDEO_SAVED:
                        var resultVideoPath = Path.Combine(_filesLocationSettings.OutputDirectory, task.ResultAnimation.Filename);

                        var thumbnail = await _thumbnailGenerator.GetThumbnailAsync(resultVideoPath);

                        task.SetStatusDone(thumbnail);
                        break;

                    case EventType.ERROR_OPENING_MODEL:
                    case EventType.ERROR_OPENING_VIDEO:
                    case EventType.ERROR_OPENING_IMAGE:
                        throw new ProcessingAnimationFailedException(ev.EventType.Text, ev.Filename);

                    case EventType.ERROR_ARGUMENT_PARSING:
                        throw new ProcessingAnimationFailedException(ev.EventType.Text);

                    case { } when ev.EventType.IsError:
                        throw new ProcessingAnimationFailedException("Unknown error when processing animation occured");
                }
            }

            _logger.LogInformation("Processing {ResultFilename} finished", task.ResultAnimation.Filename);
        }

        protected IOptionsCommandBuilder GetPreconfiguredBuilder()
        {
            if (_animatorSettings.IsUsingDocker)
            {
                if (_animatorSettings.DockerImage is null)
                    throw new MissingConfigurationSetting(nameof(_animatorSettings.DockerImage), nameof(CoreAnimatorSettings));
            }
            else
            {
                if (_animatorSettings.PythonStartingPoint is null)
                    throw new MissingConfigurationSetting(nameof(_animatorSettings.PythonStartingPoint), nameof(CoreAnimatorSettings));
            }

            var builder = (_animatorSettings.IsUsingDocker ?
                AnimationCommandBuilder.UsingDocker(_animatorSettings.DockerImage) :
                AnimationCommandBuilder.UsingPython(_animatorSettings.PythonStartingPoint))
                    .WithExecutablePath(_animatorSettings.ExecPath)
                    .WithImagesDirectory(_filesLocationSettings.ImagesDirectory)
                    .WithVideosDirectory(_filesLocationSettings.VideosDirectory)
                    .WithTempDirectory(_filesLocationSettings.TempDirectory)
                    .WithOutputDirectory(_filesLocationSettings.OutputDirectory)
                    .WithParameters();

            if (_animatorSettings.UsingGPU)
                builder.WithGPUSupport();

            return builder;
        }

        protected virtual async IAsyncEnumerable<OutputEvent> RunAnimationProcessAsync((string path, string arguments) command)
        {
            const int QUEUE_OUTPUT_DELAY_MS = 1000;

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command.path,
                    Arguments = command.arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true,
            };

            var eventsQueue = new Queue<OutputEvent>();

            var processCloseEvent = new TaskCompletionSource();
            process.Exited += (_, e) => processCloseEvent.TrySetResult();

            DataReceivedEventHandler handleOutputEvent(TaskCompletionSource closeEvent)
                => (_, e) =>
                {
                    if (e.Data is null)
                        closeEvent.TrySetResult();
                    else
                    {
                        try
                        {
                            var outputEvent = JsonSerializer.Deserialize<OutputEvent>(e.Data);
                            eventsQueue.Enqueue(outputEvent);
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Returned non-JSON output: {Output}", e.Data);
                            throw new ProcessingAnimationFailedException("Process did not return proper outputevent JSON", ex, e.Data);
                        }
                    }
                };


            var outputCloseEvent = new TaskCompletionSource();
            process.OutputDataReceived += handleOutputEvent(outputCloseEvent);

            var errorCloseEvent = new TaskCompletionSource();
            process.ErrorDataReceived += handleOutputEvent(errorCloseEvent);

            var isProcessStarted = process.Start();
            if (!isProcessStarted)
                throw new AnimatorConnectionException();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var processFinishedTask = Task.WhenAll(
                processCloseEvent.Task,
                outputCloseEvent.Task,
                errorCloseEvent.Task);

            while (await Task.WhenAny(processFinishedTask, Task.Delay(QUEUE_OUTPUT_DELAY_MS)) != processFinishedTask)
            {
                while (eventsQueue.Count > 0)
                    yield return eventsQueue.Dequeue();
            }

            while (eventsQueue.Count > 0)
                yield return eventsQueue.Dequeue();
        }

        protected class OutputEvent
        {
            public EventType EventType { get; init; }
            public double Time { get; init; }
            public string Filename { get; init; }

            public override string ToString()
                => $"OutputEvent: {{EventType:{{{EventType}}}, Time:{Time}, Filename:{Filename}}}";
        }

        protected class EventType
        {
            public bool IsError { get; init; }
            public string Name { get; init; }
            public string Text { get; init; }

            #region EventTypes
            public const string OPENING_MODEL = "OPENING_MODEL";
            public const string PROCESSING_STARTED = "PROCESSING_STARTED";
            public const string OPENING_VIDEO = "OPENING_VIDEO";
            public const string VIDEO_OPENED = "VIDEO_OPENED";
            public const string PREPROCESSING_FIND_BEST_FRAME = "PREPROCESSING_FIND_BEST_FRAME";
            public const string OPENING_VIDEO_TEMP = "OPENING_VIDEO_TEMP";
            public const string PREPROCESSING_FIND_BEST_FRAME_TEMP = "PREPROCESSING_FIND_BEST_FRAME_TEMP";
            public const string PROCESSING_VIDEO_STARTED = "PROCESSING_VIDEO_STARTED";
            public const string SAVING_OUTPUT_VIDEO = "SAVING_OUTPUT_VIDEO";
            public const string VIDEO_SAVED = "VIDEO_SAVED";


            public const string ERROR_OPENING_IMAGE = "ERROR_OPENING_IMAGE";
            public const string ERROR_OPENING_VIDEO = "ERROR_OPENING_VIDEO";
            public const string ERROR_OPENING_MODEL = "ERROR_OPENING_MODEL";
            public const string ERROR_ARGUMENT_PARSING = "ERROR_ARGUMENT_PARSING";
            #endregion

            public override string ToString()
                => $"EventType: {{IsError:{IsError}, Time:{Name}, Filename:{Text}}}";
        }

        public class AnimationCommandBuilder : IEnviromentBuilder, IOptionsCommandBuilder
        {
            private enum EnviromentTypes { Python, Docker }

            private EnviromentTypes _type = default;
            private string _startingPythonFile = default;

            private string _executablePath = null;
            private string _imagesDirectoryPath = null;
            private string _videosDirectoryPath = null;
            private string _tempDirectoryPath = null;
            private string _outputDirectoryPath = null;

            private List<Image> _images = new List<Image>();
            private List<ResultVideo> _resultVideos = new List<ResultVideo>();
            private DrivingVideo _video = default;

            private bool _withAudio = false;
            private float? _customImagePadding = null;
            private bool _withGPUSupport = false;
            private bool _withCleanBuildFlag = false;

            public (string path, string arguments) Build()
            {
                var argumentsBuilder = new StringBuilder();

                if (_type == EnviromentTypes.Docker)
                {
                    argumentsBuilder.Append(" run --rm ");

                    if (_withGPUSupport)
                        argumentsBuilder.Append(" --gpus all ");

                    if (_imagesDirectoryPath is null)
                        throw new RequiredParameterMissingException(nameof(_imagesDirectoryPath));
                    if (_videosDirectoryPath is null)
                        throw new RequiredParameterMissingException(nameof(_videosDirectoryPath));
                    if (_outputDirectoryPath is null)
                        throw new RequiredParameterMissingException(nameof(_outputDirectoryPath));
                    if (_tempDirectoryPath is null)
                        throw new RequiredParameterMissingException(nameof(_tempDirectoryPath));

                    argumentsBuilder.Append($" -v \"{_imagesDirectoryPath}\":/AlterEgo-core/images ");
                    argumentsBuilder.Append($" -v \"{_videosDirectoryPath}\":/AlterEgo-core/videos ");
                    argumentsBuilder.Append($" -v \"{_outputDirectoryPath}\":/AlterEgo-core/output ");
                    argumentsBuilder.Append($" -v \"{_tempDirectoryPath}\":/AlterEgo-core/temp ");

                    argumentsBuilder.Append($" {_startingPythonFile} ");

                    argumentsBuilder.Append(" python3 ");
                }

                argumentsBuilder.Append($" \"{(_type == EnviromentTypes.Python ? _startingPythonFile : "run.py")}\" ");

                if (_video is null)
                    throw new RequiredParameterMissingException(nameof(_video));

                argumentsBuilder.Append($" --driving_video \"{(_type == EnviromentTypes.Python ? Path.Combine(_videosDirectoryPath, _video.Filename) : _video.Filename)}\" ");

                _images.Aggregate(argumentsBuilder.Append(" --source_image "),
                    (acc, image) => acc.Append($" \"{(_type == EnviromentTypes.Python ? Path.Combine(_imagesDirectoryPath, image.Filename) : image.Filename)}\" "));
                _resultVideos.Aggregate(argumentsBuilder.Append(" --result_video "),
                    (acc, video) => acc.Append($" \"{(_type == EnviromentTypes.Python ? Path.Combine(_outputDirectoryPath, video.Filename) : video.Filename)}\" "));

                if (_withAudio)
                    argumentsBuilder.Append(" --audio ");

                if (_withCleanBuildFlag)
                    argumentsBuilder.Append(" --clean_build ");

                if (_customImagePadding.HasValue)
                    argumentsBuilder.Append($" --image_padding {_customImagePadding} ");

                if (_withGPUSupport)
                    argumentsBuilder.Append(" --crop --find_best_frame");

                argumentsBuilder.Append(" --api ");

                return (_executablePath, argumentsBuilder.ToString());
            }

            private AnimationCommandBuilder(EnviromentTypes type)
                => _type = type;

            private AnimationCommandBuilder(EnviromentTypes type, string startPointPath) : this(type)
                => _startingPythonFile = startPointPath;

            public static IEnviromentBuilder UsingPython(string corePath)
                => new AnimationCommandBuilder(EnviromentTypes.Python, Path.GetFullPath(corePath));

            public static IEnviromentBuilder UsingDocker(string dockerImageName)
                => new AnimationCommandBuilder(EnviromentTypes.Docker, dockerImageName);

            public IOptionsCommandBuilder AddResultAnimation(Image image, ResultVideo resultVideo)
            {
                if (image is null)
                    throw new ArgumentNullException(nameof(image));
                if (resultVideo is null)
                    throw new ArgumentNullException(nameof(resultVideo));

                _images.Add(image);
                _resultVideos.Add(resultVideo);

                return this;
            }

            public IOptionsCommandBuilder WithAudio()
            {
                _withAudio = true;
                return this;
            }

            public IOptionsCommandBuilder WithCustomImagePadding(float padding)
            {
                _customImagePadding = padding;
                return this;
            }

            public IOptionsCommandBuilder WithDrivingVideo(DrivingVideo video)
            {
                if (video is null)
                    throw new ArgumentNullException(nameof(video));

                _video = video;
                return this;
            }

            public IEnviromentBuilder WithExecutablePath()
                => WithExecutablePath(_type == EnviromentTypes.Docker ?
                    "docker" :
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "python" : "python3");

            public IEnviromentBuilder WithExecutablePath(string path)
            {
                if (path is null)
                    return WithExecutablePath();

                _executablePath = path;
                return this;
            }

            public IOptionsCommandBuilder WithGPUSupport()
            {
                _withGPUSupport = true;
                return this;
            }

            public IEnviromentBuilder WithImagesDirectory(string path)
            {
                _imagesDirectoryPath = path;
                return this;
            }

            public IEnviromentBuilder WithOutputDirectory(string path)
            {
                _outputDirectoryPath = path;
                return this;
            }

            public IOptionsCommandBuilder WithoutUsingPreviousTempData()
            {
                _withCleanBuildFlag = true;
                return this;
            }

            public IOptionsCommandBuilder WithParameters()
            {
                return this;
            }

            public IEnviromentBuilder WithTempDirectory(string path)
            {
                _tempDirectoryPath = path;
                return this;
            }

            public IEnviromentBuilder WithVideosDirectory(string path)
            {
                _videosDirectoryPath = path;
                return this;
            }
        }

        public interface IEnviromentBuilder
        {
            IEnviromentBuilder WithExecutablePath();
            IEnviromentBuilder WithExecutablePath(string path);

            IEnviromentBuilder WithImagesDirectory(string path);
            IEnviromentBuilder WithVideosDirectory(string path);
            IEnviromentBuilder WithOutputDirectory(string path);
            IEnviromentBuilder WithTempDirectory(string path);

            IOptionsCommandBuilder WithParameters();
        }

        public interface IOptionsCommandBuilder
        {
            IOptionsCommandBuilder WithDrivingVideo(DrivingVideo video);
            IOptionsCommandBuilder AddResultAnimation(Image image, ResultVideo resultVideo);
            IOptionsCommandBuilder WithGPUSupport();
            IOptionsCommandBuilder WithCustomImagePadding(float padding);
            IOptionsCommandBuilder WithAudio();
            IOptionsCommandBuilder WithoutUsingPreviousTempData();

            (string path, string arguments) Build();
        }
    }
}
