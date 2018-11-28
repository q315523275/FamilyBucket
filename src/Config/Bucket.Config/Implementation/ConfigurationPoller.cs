using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bucket.Config.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bucket.Config.Implementation
{
    public class ConfigurationPoller : IHostedService, IDisposable
    {
        private readonly BucketConfigOptions _setting;
        private readonly IDataRepository _dataRepository;
        private readonly IDataListener  _dataListener;
        private readonly ILogger<ConfigurationPoller> _logger;
        private Timer _timer;
        private bool _polling;

        public ConfigurationPoller(IOptions<BucketConfigOptions> setting, IDataRepository dataRepository, IDataListener dataListener, ILogger<ConfigurationPoller> logger)
        {
            _setting = setting.Value;
            _dataRepository = dataRepository;
            _dataListener = dataListener;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(Welcome());

            if(_setting.RefreshInteval == 0)
                return Task.CompletedTask;

            _logger.LogInformation($"Bucket Config {nameof(ConfigurationPoller)} Is Starting.");

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

            _dataListener.AddListener();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Bucket Config {nameof(ConfigurationPoller)} Is Stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task Poll()
        {
            _logger.LogInformation("Bucket Config Started Polling");

            await _dataRepository.Get();

            _logger.LogInformation("Bucket Config Finished Polling");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private string Welcome()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Initializing ...");
            builder.AppendLine();
            builder.AppendLine("***************************************************************");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("*                  Welcome To Bucket Config                   *");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("***************************************************************");
            return builder.ToString();
        }
    }
}
