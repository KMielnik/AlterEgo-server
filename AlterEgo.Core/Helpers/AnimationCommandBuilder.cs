using AlterEgo.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Helpers
{
    public class AnimationCommandBuilder : IEnviromentBuilder, IDrivingVideoCommandBuilder, IOptionsCommandBuilder
    {
        public static IEnviromentBuilder UsingPython()
        {
            return new AnimationCommandBuilder();
        }

        public static IEnviromentBuilder UsingDocker(string dockerImageName)
        {
            return new AnimationCommandBuilder();
        }

        public IOptionsCommandBuilder AddResultAnimation(Image image, ResultVideo resultVideo)
        {
            throw new NotImplementedException();
        }

        public (string path, string arguments) Build()
        {
            throw new NotImplementedException();
        }

        public IOptionsCommandBuilder WithAudio()
        {
            throw new NotImplementedException();
        }

        public IOptionsCommandBuilder WithCustomImagePadding(float padding)
        {
            throw new NotImplementedException();
        }

        public IOptionsCommandBuilder WithDrivingVideo(DrivingVideo video)
        {
            throw new NotImplementedException();
        }

        public IEnviromentBuilder WithExecutablePath(string path)
        {
            throw new NotImplementedException();
        }

        public IOptionsCommandBuilder WithGPUSupport()
        {
            throw new NotImplementedException();
        }

        public IEnviromentBuilder WithImagesDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public IEnviromentBuilder WithOutputDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public IOptionsCommandBuilder WithoutUsingPreviousTempData()
        {
            throw new NotImplementedException();
        }

        public IDrivingVideoCommandBuilder WithParameters()
        {
            throw new NotImplementedException();
        }

        public IEnviromentBuilder WithTempDirectory(string path)
        {
            throw new NotImplementedException();
        }

        public IEnviromentBuilder WithVideosDirectory(string path)
        {
            throw new NotImplementedException();
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
