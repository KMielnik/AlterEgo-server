using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Services
{
    public class Animator : IAnimator
    {
        public async Task<AnimationTask> Animate(AnimationTask task)
        {
            var user = new User("login", "password", "salt", "Agatka", "elo@wp.pl");
            var video = new DrivingVideo("a.mp4", user, TimeSpan.Zero);
            var image = new Image("a.jpg", user, TimeSpan.Zero);
            var image2 = new Image("a.jpg", user, TimeSpan.Zero);
            var result = new ResultVideo("Output.mp4", user, TimeSpan.Zero);
            var result2 = new ResultVideo("outputto.mp4", user, TimeSpan.Zero);

            var elo = AnimationCommandBuilder.UsingDocker("kamilmielnik/alterego-core:2.0.4")
                .WithExecutablePath()
                .WithImagesDirectory("images")
                .WithVideosDirectory("videos")
                .WithTempDirectory("temp")
                .WithOutputDirectory("out put")
                .WithParameters()
                .WithDrivingVideo(video)
                .AddResultAnimation(image, result)
                .WithAudio()
                .Build();
            Console.WriteLine(elo.path + " " + elo.arguments);

            await foreach(var el in RunAnimationProcessAsync(elo))
            {
                Console.WriteLine(el.EventType.Text);
            }

            return task;
        }

        private async IAsyncEnumerable<OutputEvent> RunAnimationProcessAsync((string path, string arguments) command, int? timeout = null)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command.path,
                    Arguments = command.arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            var eventsQueue = new Queue<OutputEvent>();
            
            var processCloseEvent = new TaskCompletionSource();
            process.Exited += (_, e) => processCloseEvent.TrySetResult();

            Func<TaskCompletionSource, DataReceivedEventHandler> handleOutputEvent = (closeEvent) =>
            {
                return (_, e) =>
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
                            //TODO: exceptions in animator
                            Console.WriteLine(ex.Message);
                        }
                    }
                };
            };

            var outputCloseEvent = new TaskCompletionSource();
            process.OutputDataReceived += handleOutputEvent(outputCloseEvent);

            var errorCloseEvent = new TaskCompletionSource();
            process.ErrorDataReceived += handleOutputEvent(errorCloseEvent);

            var isProcessStarted = process.Start();
            if (!isProcessStarted)
                throw new Exception("fail");

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var processFinishedTask = Task.WhenAll(
                processCloseEvent.Task,
                outputCloseEvent.Task,
                errorCloseEvent.Task);

            var awaitingTask = timeout.HasValue
                ? Task.WhenAny(Task.Delay(timeout.Value), processFinishedTask)
                : Task.WhenAny(processFinishedTask);

            if (await awaitingTask == processFinishedTask)
                while (eventsQueue.Count > 0)
                    yield return eventsQueue.Dequeue();
            else
                process.Kill();
        }

        private class OutputEvent
        {
            public EventType EventType { get; init; }
            public double Time { get; init; }
            public string Filename { get; init; }
        }

        private class EventType
        {
            public bool IsError { get; init; }
            public string Name { get; init; }
            public string Text { get; init; }
        }

        public class AnimationCommandBuilder : IEnviromentBuilder, IDrivingVideoCommandBuilder, IOptionsCommandBuilder
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
                    argumentsBuilder.Append(" run -it --rm ");

                    if (_withGPUSupport)
                        argumentsBuilder.Append(" --gpus all ");

                    if (_imagesDirectoryPath is null ||
                        _videosDirectoryPath is null ||
                        _outputDirectoryPath is null ||
                        _tempDirectoryPath is null)
                        throw new Exception();

                    argumentsBuilder.Append($" -v \"{_imagesDirectoryPath}\":/AlterEgo-core/images ");
                    argumentsBuilder.Append($" -v \"{_videosDirectoryPath}\":/AlterEgo-core/videos ");
                    argumentsBuilder.Append($" -v \"{_outputDirectoryPath}\":/AlterEgo-core/output ");
                    argumentsBuilder.Append($" -v \"{_tempDirectoryPath}\":/AlterEgo-core/temp ");

                    argumentsBuilder.Append($" {_startingPythonFile} ");

                    argumentsBuilder.Append(" python3 ");
                }

                argumentsBuilder.Append($" \"{(_type == EnviromentTypes.Python ? _startingPythonFile : "run.py")}\" ");

                if (_video is null)
                    throw new Exception();

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
                _video = video;
                return this;
            }

            public IEnviromentBuilder WithExecutablePath()
                => this.WithExecutablePath(_type == EnviromentTypes.Docker ?
                    "docker" :
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "python" : "python3");

            public IEnviromentBuilder WithExecutablePath(string path)
            {
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
                _imagesDirectoryPath = Path.GetFullPath(path);
                return this;
            }

            public IEnviromentBuilder WithOutputDirectory(string path)
            {
                _outputDirectoryPath = Path.GetFullPath(path);
                return this;
            }

            public IOptionsCommandBuilder WithoutUsingPreviousTempData()
            {
                _withCleanBuildFlag = true;
                return this;
            }

            public IDrivingVideoCommandBuilder WithParameters()
            {
                return this;
            }

            public IEnviromentBuilder WithTempDirectory(string path)
            {
                _tempDirectoryPath = Path.GetFullPath(path);
                return this;
            }

            public IEnviromentBuilder WithVideosDirectory(string path)
            {
                _videosDirectoryPath = Path.GetFullPath(path);
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

            IDrivingVideoCommandBuilder WithParameters();
        }

        public interface IDrivingVideoCommandBuilder
        {
            IOptionsCommandBuilder WithDrivingVideo(DrivingVideo video);
        }

        public interface IOptionsCommandBuilder
        {
            IOptionsCommandBuilder AddResultAnimation(Image image, ResultVideo resultVideo);
            IOptionsCommandBuilder WithGPUSupport();
            IOptionsCommandBuilder WithCustomImagePadding(float padding);
            IOptionsCommandBuilder WithAudio();
            IOptionsCommandBuilder WithoutUsingPreviousTempData();

            (string path, string arguments) Build();
        }
    }
}
