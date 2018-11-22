using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Authorize.MySql
{
    public class MySqlPermissionPoller : IHostedService, IDisposable
    {
        private readonly IConfiguration configuration;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<MySqlPermissionPoller> _logger;
        private Timer _timer;
        private bool _polling;

        public MySqlPermissionPoller(IConfiguration configuration, IPermissionRepository permissionRepository, ILogger<MySqlPermissionPoller> logger)
        {
            this.configuration = configuration;
            _permissionRepository = permissionRepository;
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(Welcome());

            _logger.LogInformation($"{nameof(MySqlPermissionPoller)} is starting.");

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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(MySqlPermissionPoller)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task Poll()
        {
            _logger.LogInformation("Started polling");

            await _permissionRepository.Get();

            _logger.LogInformation("Finished polling");
        }


        private string Welcome()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Initializing ...");
            builder.AppendLine();
            builder.AppendLine("***************************************************************");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("*                 Welcome to Bucket Authorize                 *");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("***************************************************************");
            return builder.ToString();
        }

    }
}
