using Bucket.HostedService;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Authorize.HostedService
{
    public class BucketAuthorizeHostedService : IExecutionService, IDisposable
    {
        private readonly IConfiguration configuration;
        private readonly IPermissionRepository _permissionRepository;
        private Timer _timer;
        private bool _polling;

        public BucketAuthorizeHostedService(IConfiguration configuration, IPermissionRepository permissionRepository)
        {
            this.configuration = configuration;
            _permissionRepository = permissionRepository;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var config = configuration.GetSection("JwtAuthorize");

            var delay = Convert.ToInt32(config["RefreshInteval"]) * 1000;

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
            await _permissionRepository.Get();
        }
    }
}
