using Bucket.Core;
using Bucket.Listener.Abstractions;
using Bucket.Redis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Bucket.Listener.Redis
{
    public class RedisListenerAgentStartup : IListenerAgentStartup
    {
        private readonly RedisClient _redisClient;
        private readonly RedisListenerOptions _redisListenerOptions;
        private readonly string RedisListenerKey;
        private ISubscriber _subscriber;
        private readonly IExtractCommand _extractCommand;
        public RedisListenerAgentStartup(RedisClient redisClient, IOptions<RedisListenerOptions> redisListenerOptions, IExtractCommand extractCommand)
        {
            _redisClient = redisClient;
            _redisListenerOptions = redisListenerOptions.Value;
            _extractCommand = extractCommand;
            RedisListenerKey = $"Bucket.Listener.{_redisListenerOptions.ListenerKey}";
        }

        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _subscriber = _redisClient.GetSubscriber(_redisListenerOptions.ConnectionString);
            return _subscriber.SubscribeAsync(RedisListenerKey, (channel, message) =>
            {
                var command = JsonConvert.DeserializeObject<Bucket.Values.NetworkCommand>(message);
                _extractCommand.CommandNotify(command);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_subscriber != null)
            {
                return _subscriber.UnsubscribeAsync(RedisListenerKey);
            }
            return Task.CompletedTask;
        }
    }
}
