using Bucket.Listener.Abstractions;
using Bucket.Values;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rabbit.Zookeeper;
using Rabbit.Zookeeper.Implementation;
using System.Text;
using System.Threading.Tasks;
namespace Bucket.Listener.Zookeeper
{
    public class ZookeeperPublishCommand : IPublishCommand
    {
        private readonly IZookeeperClient _client;
        private readonly ZookeeperListenerOptions _options;
        public ZookeeperPublishCommand(IOptions<ZookeeperListenerOptions> options)
        {
            _options = options.Value;
            _client = new ZookeeperClient(_options);
        }

        public async Task PublishCommandMessage(string applicationCode, NetworkCommand command)
        {
            var ListenerPath = $"/Bucket.Listener/{applicationCode}";
            var existPath = await _client.ExistsAsync(ListenerPath);
            if (!existPath)
                await _client.CreateRecursiveAsync(ListenerPath, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command)));
            await _client.SetDataAsync(ListenerPath, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command)));
        }
    }
}
