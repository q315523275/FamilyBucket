using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bucket.ErrorCode
{
    public class ErrorCodePoller : IHostedService, IDisposable
    {
        private readonly ErrorCodeSetting _errorCodeConfiguration;
        private readonly IDataRepository _dataRepository;
        private readonly ILogger<ErrorCodePoller> _logger;
        private Timer _timer;
        private bool _polling;

        public ErrorCodePoller(IOptions<ErrorCodeSetting> errorCodeConfiguration, IDataRepository dataRepository, ILogger<ErrorCodePoller> logger)
        {
            _errorCodeConfiguration = errorCodeConfiguration.Value;
            _dataRepository = dataRepository;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(Welcome());

            if(_errorCodeConfiguration.RefreshInteval == 0)
                return Task.CompletedTask;

            _logger.LogInformation($"Bucket ErrorCode {nameof(ErrorCodePoller)} Is Starting.");

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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Bucket ErrorCode {nameof(ErrorCodePoller)} Is Stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task Poll()
        {
            _logger.LogInformation("Bucket ErrorCode Started Polling");

            await _dataRepository.Get();

            _logger.LogInformation("Bucket ErrorCode Finished Polling");
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
            builder.AppendLine("*                 Welcome To Bucket ErrorCode                 *");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("***************************************************************");
            return builder.ToString();
        }
    }
}
