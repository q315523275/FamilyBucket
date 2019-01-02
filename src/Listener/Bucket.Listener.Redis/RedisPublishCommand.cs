using System.Threading.Tasks;
using Bucket.Listener.Abstractions;
using Bucket.Redis;
using Bucket.Values;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
namespace Bucket.Listener.Redis
{
    public class RedisPublishCommand: IPublishCommand
    {
        private readonly RedisClient _redisClient;
        private readonly RedisListenerOptions _redisListenerOptions;
        public RedisPublishCommand(RedisClient redisClient, IOptions<RedisListenerOptions> redisListenerOptions)
        {
            _redisClient = redisClient;
            _redisListenerOptions = redisListenerOptions.Value;
        }

        public async Task PublishCommandMessage(string applicationCode, NetworkCommand command)
        {
            var redis = _redisClient.GetSubscriber(_redisListenerOptions.ConnectionString);
            await redis.PublishAsync($"Bucket.Listener.{applicationCode}", JsonConvert.SerializeObject(command));
        }
    }
}
