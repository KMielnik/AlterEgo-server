﻿using AlterEgo.Core.Interfaces;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
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

            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _tasksRepository = scope.ServiceProvider.GetRequiredService<IAnimationTaskRepository>();

                _logger.LogTrace("Getting tasks from database");
                var newTasks = _tasksRepository
                    .GetAllAsync()
                    .Where(t => t.Status == Core.Domains.AnimationTask.Statuses.New)
                    .OrderBy(t => t.CreatedAt);

                await foreach (var task in newTasks)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    task.SetStatusProcessing();

                    _logger.LogDebug("Received {@Task}, starting processing", task);
                    try
                    {
                        await _animator.Animate(task);
                        //TODO: Notify user about completed task.
                    }
                    catch (MissingConfigurationSetting ex)
                    {
                        _logger.LogCritical(ex, "Missing settings in Animator, cancelling processing.");
                        task.SetStatusFailed();

                        throw;
                    }
                    catch (ProcessingAnimationFailedException ex)
                    {
                        _logger.LogError(ex, "Failed processing task.");
                        task.SetStatusFailed();
                    }
                    catch (AnimatorConnectionException ex)
                    {
                        _logger.LogCritical(ex, "Couldn't connect to animator.");
                        task.SetStatusFailed();

                        throw;
                    }
                    catch (RequiredParameterMissingException ex)
                    {
                        _logger.LogCritical(ex, "Animator builder didn't receive all parameters");
                        task.SetStatusFailed();

                        throw;
                    }
                    catch(FileNotFoundException ex)
                    {
                        _logger.LogCritical(ex, "Couldn't open file from task");
                        task.SetStatusFailed();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Couldn't process task for unkown reason");
                        task.SetStatusFailed();

                        throw;
                    }
                    finally
                    {
                        await _tasksRepository.UpdateAsync(task);

                        _logger.LogDebug("Task {@Task} processing done", task);
                    }
                }

                await Task.Delay(5000, cancellationToken);
            }

            _logger.LogDebug("Animation tasks service is stopping");
        }
    }
}
