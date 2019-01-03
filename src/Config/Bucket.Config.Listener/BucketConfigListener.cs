using Bucket.Listener;
using System.Threading.Tasks;
using Bucket.Config.Abstractions;
using Bucket.Values;
using System;

namespace Bucket.Config.Listener
{
    public class BucketConfigListener : IBucketListener
    {
        public string ListenerName => "Bucket.Config";

        private readonly IDataRepository _dataRepository;

        public BucketConfigListener(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task ExecuteAsync(string commandText)
        {
            if (!string.IsNullOrWhiteSpace(commandText) && commandText == NetworkCommandType.Refresh.ToString())
                await _dataRepository.Get();
            if (!string.IsNullOrWhiteSpace(commandText) && commandText == NetworkCommandType.Reload.ToString())
                await _dataRepository.Get(true);
        }
    }
}
