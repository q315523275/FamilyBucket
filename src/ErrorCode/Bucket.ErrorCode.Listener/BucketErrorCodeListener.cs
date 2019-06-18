using Bucket.ErrorCode.Abstractions;
using Bucket.Listener;
using Bucket.Values;
using System.Threading.Tasks;

namespace Bucket.ErrorCode.Listener
{
    public class BucketErrorCodeListener : IBucketListener
    {
        public string ListenerName => "Bucket.ErrorCode";

        private readonly IDataRepository _dataRepository;

        public BucketErrorCodeListener(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task ExecuteAsync(string commandText)
        {
            if (!string.IsNullOrWhiteSpace(commandText) && commandText == NetworkCommandType.Reload.ToString())
                await _dataRepository.Get();
        }
    }
}
