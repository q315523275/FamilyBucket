using Bucket.Listener.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Rabbit.Zookeeper;
using org.apache.zookeeper;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Rabbit.Zookeeper.Implementation;
using Newtonsoft.Json;
namespace Bucket.Listener.Zookeeper
{
    public class ZookeeperListenerAgentStartup : IListenerAgentStartup
    {
        private readonly IZookeeperClient _client;
        private readonly ZookeeperListenerOptions _options;
        private readonly IExtractCommand _extractCommand;
        private readonly string ListenerPath;
        private bool _isSubscribe = false;
        public ZookeeperListenerAgentStartup(IOptions<ZookeeperListenerOptions> options, IExtractCommand extractCommand)
        {
            _extractCommand = extractCommand;
            _options = options.Value;
            _client = new ZookeeperClient(_options);
            ListenerPath = $"/Bucket.Listener/{_options.ListenerKey}";
        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var existPath = await _client.ExistsAsync(ListenerPath);
            if (!existPath)
                await _client.CreateRecursiveAsync(ListenerPath, Encoding.UTF8.GetBytes("Bucket.Listener"));
            await _client.SubscribeDataChange(ListenerPath, async (ct, args) =>
            {
                if (!_isSubscribe)
                    return;
                var currentData = Encoding.UTF8.GetString(args.CurrentData.ToArray());
                if (!string.IsNullOrWhiteSpace(currentData))
                {
                    var command = JsonConvert.DeserializeObject<Values.NetworkCommand>(currentData);
                    if (args.Path.ToLower() == ListenerPath.ToLower())
                    {
                        switch (args.Type)
                        {
                            case Watcher.Event.EventType.NodeDataChanged:
                                await _extractCommand.ExtractCommandMessage(command);
                                break;
                        }
                    }
                }
            });
            _isSubscribe = true;
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_client != null && !cancellationToken.IsCancellationRequested)
                _client.UnSubscribeDataChange(ListenerPath, (ct, args) => { return Task.CompletedTask; });
            return Task.CompletedTask;
        }
    }
}
