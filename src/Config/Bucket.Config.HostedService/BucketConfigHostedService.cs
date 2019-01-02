using Bucket.Config.Abstractions;
using Bucket.HostedService;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Config.HostedService
{
    public class BucketConfigHostedService : IExecutionService, IDisposable
    {
        private readonly BucketConfigOptions _setting;
        private readonly IDataRepository _dataRepository;
        private Timer _timer;
        private bool _polling;

        public BucketConfigHostedService(IOptions<BucketConfigOptions> setting, IDataRepository dataRepository)
        {
            _setting = setting.Value;
            _dataRepository = dataRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_setting.RefreshInteval == 0)
                return Task.CompletedTask;

            var delay = _setting.RefreshInteval * 1000;
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

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
