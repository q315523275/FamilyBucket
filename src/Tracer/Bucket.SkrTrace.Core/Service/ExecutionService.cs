using System;
using System.Threading;
using System.Threading.Tasks;
using Bucket.SkrTrace.Core.Abstractions;

namespace Bucket.SkrTrace.Core.Service
{
    public abstract class ExecutionService : IExecutionService, IDisposable
    {
        private Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        protected readonly IRuntimeEnvironment RuntimeEnvironment;

        public ExecutionService(IRuntimeEnvironment runtimeEnvironment)
        {
            RuntimeEnvironment = runtimeEnvironment;
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var source = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            _timer = new Timer(Callback, source, DueTime, Period);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _cancellationTokenSource?.Cancel();
            await Stopping(cancellationToken);
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async void Callback(object state)
        {
            if (state is CancellationTokenSource token && !token.IsCancellationRequested && CanExecute())
            {
                await ExecuteAsync(token.Token);
            }
        }
        protected virtual Task Stopping(CancellationToken cancellationToke) => Task.CompletedTask;
        protected abstract TimeSpan DueTime { get; }
        protected abstract TimeSpan Period { get; }
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
        protected virtual bool CanExecute() => RuntimeEnvironment.Initialized;
    }
}
