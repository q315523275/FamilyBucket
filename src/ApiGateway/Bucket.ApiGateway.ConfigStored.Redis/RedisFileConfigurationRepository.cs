using System.Threading.Tasks;
using Bucket.Redis;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;

namespace Bucket.ApiGateway.ConfigStored.Redis
{
    public class RedisFileConfigurationRepository: IFileConfigurationRepository
    {
        private readonly RedisClient _redisClient;
        private readonly string _apiGatewayKey;
        private readonly string _redisConnection;

        public RedisFileConfigurationRepository(RedisClient redisClient, string apiGatewayKey, string redisConnection)
        {
            _redisClient = redisClient;
            _apiGatewayKey = apiGatewayKey;
            _redisConnection = redisConnection;
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            var redis = _redisClient.GetDatabase(_redisConnection);

            var json = await redis.StringGetAsync($"ApiGatewayConfig:{_apiGatewayKey}");

            if(json.IsNullOrEmpty)
                return new OkResponse<FileConfiguration>(new FileConfiguration { });

            var fileConfig = JsonConvert.DeserializeObject<FileConfiguration>(json);

            return new OkResponse<FileConfiguration>(fileConfig);
        }

        public async Task<Response> Set(FileConfiguration fileConfiguration)
        {
            return await Task.FromResult(new OkResponse());
        }
    }
}
