using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Animation;
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

namespace AlterEgo.Infrastructure.Services.Animation.BackgroundServices
{
    public class FakeAnimator : IAnimator
    {
        private readonly FakeAnimatorSettings _animatorSettings;
        private readonly FilesLocationSettings _filesLocationSettings;
        private readonly ILogger<FakeAnimator> _logger;
        private readonly Random _randomGenerator;

        public FakeAnimator(
            IOptions<FakeAnimatorSettings> animatorSettings,
            IOptions<FilesLocationSettings> filesLocationSettings,
            ILogger<FakeAnimator> logger)
        {
            _animatorSettings = animatorSettings.Value;
            _filesLocationSettings = filesLocationSettings.Value;

            _logger = logger;
            _randomGenerator = new Random();
        }

        public async Task Animate(AnimationTask task)
        {
            _logger.LogInformation("Started simulating animation of task with outputfile - {OutputFile}", task.ResultAnimation.Filename);
            _logger.LogDebug("Processed task details - {@Task}", task);

            int waitTime =
                (int)((_randomGenerator.NextDouble() * (_animatorSettings.MaxProcessingTime - _animatorSettings.MinProcessingTime)
                + _animatorSettings.MinProcessingTime)
                * 1000);

            _logger.LogInformation("Planned processing time (in ms) {WaitTime}", waitTime);

            if (_animatorSettings.ShouldCopyVideo)
            {
                _logger.LogInformation("Selected waiting and copying video to simulate creation.");
                if (_filesLocationSettings.VideosDirectory is null)
                    throw new MissingConfigurationSetting(nameof(_filesLocationSettings.VideosDirectory), nameof(FakeAnimatorSettings));
                if (_filesLocationSettings.OutputDirectory is null)
                    throw new MissingConfigurationSetting(nameof(_filesLocationSettings.OutputDirectory), nameof(FakeAnimatorSettings));

                using var sourceStream = File.OpenRead(Path.Combine(_filesLocationSettings.VideosDirectory, task.SourceVideo.Filename));
                using var outputStream = File.Create(Path.Combine(_filesLocationSettings.OutputDirectory, task.ResultAnimation.Filename));

                await Task.WhenAll(sourceStream.CopyToAsync(outputStream), Task.Delay(waitTime));
            }
            else
            {
                _logger.LogInformation("Selected simple waiting as simulation.");
                await Task.Delay(waitTime);
            }

            _logger.LogInformation("Simulation done");
            task.SetStatusDone();
            _logger.LogInformation("Task processed");
        }
    }
}
