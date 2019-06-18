using Bucket.Listener.Abstractions;
using Bucket.Redis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;
namespace Bucket.Listener.Redis
{
    public class RedisListenerAgentStartup : IListenerAgentStartup
    {
        private readonly RedisClient _redisClient;
        private readonly IExtractCommand _extractCommand;
        private string RedisListenerKey;
        private RedisListenerOptions _redisListenerOptions;
        private ISubscriber _subscriber;
        public RedisListenerAgentStartup(RedisClient redisClient, IOptionsMonitor<RedisListenerOptions> redisListenerOptions, IExtractCommand extractCommand)
        {
            _redisClient = redisClient;
            _redisListenerOptions = redisListenerOptions.CurrentValue;
            _extractCommand = extractCommand;
            RedisListenerKey = $"Bucket.Listener.{_redisListenerOptions.ListenerKey}";
            redisListenerOptions.OnChange(async (options) =>
            {
                _redisListenerOptions = options;
                RedisListenerKey = $"Bucket.Listener.{_redisListenerOptions.ListenerKey}";
                await SubscribeAsync();
            });
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return SubscribeAsync();
        }
        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_subscriber != null)
            {
                return _subscriber.UnsubscribeAsync(RedisListenerKey);
            }
            return Task.CompletedTask;
        }
        public Task SubscribeAsync()
        {
            _subscriber = _redisClient.GetSubscriber(_redisListenerOptions.ConnectionString);
            return _subscriber.SubscribeAsync(RedisListenerKey, (channel, message) =>
            {
                var command = JsonConvert.DeserializeObject<Bucket.Values.NetworkCommand>(message);
                _extractCommand.ExtractCommandMessage(command);
            });
        }
    }
}
