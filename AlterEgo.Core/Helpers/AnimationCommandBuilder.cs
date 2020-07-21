using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Helpers
{
    public class AnimationCommandBuilder : IEnviromentBuilder, IDrivingVideoCommandBuilder, IOptionsCommandBuilder
    {
        private enum EnviromentTypes { Python, Docker}

        private EnviromentTypes _type = default;
        private string _dockerImageName = default;

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

                argumentsBuilder.Append($" -v {_imagesDirectoryPath}:/AlterEgo-core/images ");
                argumentsBuilder.Append($" -v {_videosDirectoryPath}:/AlterEgo-core/videos ");
                argumentsBuilder.Append($" -v {_outputDirectoryPath}:/AlterEgo-core/output ");
                argumentsBuilder.Append($" -v {_tempDirectoryPath}:/AlterEgo-core/temp ");

                argumentsBuilder.Append($" {_dockerImageName} ");

                argumentsBuilder.Append(" python3 ");
            }

            argumentsBuilder.Append(" run.py ");

            if (_video is null)
                throw new Exception();

            argumentsBuilder.Append($" --driving_video {_video.Filename} ");

            _images.Aggregate(argumentsBuilder.Append(" --source_image "), (acc, image) => acc.Append($" {(_type == EnviromentTypes.Python ? image.Filename : image.Filename)} "));
            _resultVideos.Aggregate(argumentsBuilder.Append(" --result_video "), (acc, video) => acc.Append($" {(_type == EnviromentTypes.Python ? video.Filename : video.Filename)} "));

            if (_withAudio)
                argumentsBuilder.Append(" --audio ");

            if (_withCleanBuildFlag)
                argumentsBuilder.Append(" --clean_build ");

            if (_customImagePadding.HasValue)
                argumentsBuilder.Append($" --image_padding {_customImagePadding} ");

            if(_withGPUSupport)
                argumentsBuilder.Append(" --crop --find_best_frame");


            return (_executablePath, argumentsBuilder.ToString());
        }

        private AnimationCommandBuilder(EnviromentTypes type)
            => _type = type;

        private AnimationCommandBuilder(EnviromentTypes type, string dockerImageName) : this(type)
            => _dockerImageName = dockerImageName;
        

        public static IEnviromentBuilder UsingPython()
            => new AnimationCommandBuilder(EnviromentTypes.Python);

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
