using AlterEgo.Core.Interfaces;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Services
{
    public class AnimationTasksProcessorService : BackgroundService
    {
        private readonly ILogger<AnimationTasksProcessorService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAnimator _animator;

        public AnimationTasksProcessorService(ILogger<AnimationTasksProcessorService> logger, IServiceScopeFactory scopeFactory, IAnimator animator)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _animator = animator;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("{ServiceName} started", nameof(AnimationTasksProcessorService));

            cancellationToken.Register(() => _logger.LogDebug("Animation tasks service is stopping"));

            while(!cancellationToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _tasksRepository = scope.ServiceProvider.GetRequiredService<IAnimationTaskRepository>();

                _logger.LogTrace("Getting task from database");
                var task = await _tasksRepository
                    .GetAllAsync()
                    .Where(t => t.Status == Core.Domains.AnimationTask.Statuses.New)
                    .OrderBy(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                if(task is null)
                {
                    _logger.LogTrace("No tasks left");
                }
                else
                {
                    _logger.LogDebug("Received {@Task}, starting processing", task);
                    try
                    {
                        await _animator.Animate(task);
                    }
                    catch( Exception ex)
                    {
                        _logger.LogDebug(ex, "Exce");
                        task.SetStatusFailed();
                    }
                    finally
                    {
                        await _tasksRepository.UpdateAsync(task);
                    }
                }

                await Task.Delay(1000, cancellationToken);
            }

            _logger.LogDebug("Animation tasks service is stopping");
        }
    }
}
