using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Animation;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlterEgo.Infrastructure.Services.Animation.BackgroundServices
{
    public class AnimationTasksProcessorService : BackgroundService
    {
        private readonly ILogger<AnimationTasksProcessorService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAnimator _animator;

        public AnimationTasksProcessorService(
            ILogger<AnimationTasksProcessorService> logger,
            IServiceScopeFactory scopeFactory,
            IAnimator animator,
            IHostApplicationLifetime appLifetime) : base(logger, appLifetime)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _animator = animator;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("{ServiceName} started", nameof(AnimationTasksProcessorService));

            cancellationToken.Register(() => _logger.LogDebug("Animation tasks service is stopping"));

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _tasksRepository = scope.ServiceProvider.GetRequiredService<IAnimationTaskRepository>();

                    var newTasks = await _tasksRepository
                        .GetAllAsync()
                        .Where(t => t.Status == AnimationTask.Statuses.New)
                        .OrderBy(t => t.CreatedAt)
                        .ToListAsync();

                    foreach (var task in newTasks)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        task.SetStatusProcessing();
                        await _tasksRepository.UpdateAsync(task);

                        _logger.LogDebug("Received {@Task}, starting processing", task);
                        try
                        {
                            await _animator.Animate(task);
                            //TODO: Notify user about completed task.
                        }
                        catch (MissingConfigurationSetting ex)
                        {
                            _logger.LogCritical(ex, "Missing settings in Animator, cancelling processing.");

                            throw;
                        }
                        catch (ProcessingAnimationFailedException ex)
                        {
                            _logger.LogError(ex, "Failed processing task.");
                        }
                        catch (AnimatorConnectionException ex)
                        {
                            _logger.LogCritical(ex, "Couldn't connect to animator.");

                            throw;
                        }
                        catch (RequiredParameterMissingException ex)
                        {
                            _logger.LogCritical(ex, "Animator builder didn't receive all parameters");

                            throw;
                        }
                        catch (FileNotFoundException ex)
                        {
                            _logger.LogError(ex, "Couldn't open file from task");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogCritical(ex, "Couldn't process task for unkown reason");

                            throw;
                        }
                        finally
                        {
                            bool taskFinished =
                               AnimationTask.Statuses.Done == task.Status ||
                               AnimationTask.Statuses.Notified == task.Status;

                            if (!taskFinished)
                                task.SetStatusFailed();

                            await _tasksRepository.UpdateAsync(task);

                            _logger.LogDebug("Task {@Task} status updated", task);
                        }
                    }
                }

                await Task.Delay(5000, cancellationToken);
            }

            _logger.LogDebug("Animation tasks service is stopping");
        }
    }
}
