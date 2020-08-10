using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlterEgo.Infrastucture.Services.BackgroundServices
{
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        protected BackgroundService(ILogger logger, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }

        protected abstract Task ExecuteAsync(CancellationToken cancaellationToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            CatchEventualException();

            if (_executingTask.IsCompleted)
                return _executingTask;



            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }

        private async void CatchEventualException()
        {
            try
            {
                await _executingTask;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "BackgroundService encountered unhandled exception, exiting application...");
                _appLifetime.StopApplication();
            }
        }
    }
}
