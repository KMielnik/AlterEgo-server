using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces;
using AlterEgo.Core.Settings;
using AlterEgo.Infrastucture.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Services.BackgroundServices
{
    public class FakeAnimator : IAnimator
    {
        private readonly FakeAnimatorSettings _settings;
        private readonly ILogger<FakeAnimator> _logger;
        private readonly Random _randomGenerator;

        public FakeAnimator(IOptions<FakeAnimatorSettings> settings, ILogger<FakeAnimator> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _randomGenerator = new Random();
        }

        public async Task Animate(AnimationTask task)
        {
            _logger.LogInformation("Started simulating animation of task with outputfile - {OutputFile}", task.ResultAnimation.Filename);
            _logger.LogDebug("Processed task details - {@Task}", task);

            int waitTime =
                (int)((_randomGenerator.NextDouble() * (_settings.MaxProcessingTime - _settings.MinProcessingTime)
                + _settings.MinProcessingTime)
                * 1000);

            _logger.LogInformation("Planned processing time (in ms) {WaitTime}", waitTime);

            if (_settings.ShouldCopyVideo)
            {
                _logger.LogInformation("Selected waiting and copying video to simulate creation.");
                if (_settings.VideosDirectory is null)
                    throw new MissingConfigurationSetting(nameof(_settings.VideosDirectory), nameof(FakeAnimatorSettings));
                if (_settings.OutputDirectory is null)
                    throw new MissingConfigurationSetting(nameof(_settings.OutputDirectory), nameof(FakeAnimatorSettings));

                using var sourceStream = File.OpenRead(Path.Combine(_settings.VideosDirectory, task.SourceVideo.Filename));
                using var outputStream = File.Create(Path.Combine(_settings.OutputDirectory, task.ResultAnimation.Filename));

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
