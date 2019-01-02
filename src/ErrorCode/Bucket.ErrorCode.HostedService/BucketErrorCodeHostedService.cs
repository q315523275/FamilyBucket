using Bucket.ErrorCode.Abstractions;
using Bucket.HostedService;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.ErrorCode.HostedService
{
    public class BucketErrorCodeHostedService : IExecutionService, IDisposable
    {
        private readonly ErrorCodeSetting _errorCodeConfiguration;
        private readonly IDataRepository _dataRepository;
        private Timer _timer;
        private bool _polling;

        public BucketErrorCodeHostedService(IOptions<ErrorCodeSetting> errorCodeConfiguration, IDataRepository dataRepository)
        {
            _errorCodeConfiguration = errorCodeConfiguration.Value;
            _dataRepository = dataRepository;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_errorCodeConfiguration.RefreshInteval == 0)
                return Task.CompletedTask;

            var delay = _errorCodeConfiguration.RefreshInteval * 1000;
            _timer = new Timer(async x =>
            {
                if (_polling)
                {
                    return;
                }

                _polling = true;
                await Poll();
                _polling = false;
            }, null, delay, delay);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task Poll()
        {
            await _dataRepository.Get();
        }
    }
}
