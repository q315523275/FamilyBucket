using Bucket.Listener;
using Bucket.Values;
using System;
using System.Threading.Tasks;

namespace Bucket.Authorize.Listener
{
    public class BucketAuthorizeListener : IBucketListener
    {
        private readonly IPermissionRepository _permissionRepository;

        public BucketAuthorizeListener(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public string ListenerName => "Bucket.Authorize";

        public async Task ExecuteAsync(string commandText)
        {
            if (!string.IsNullOrWhiteSpace(commandText) && commandText == NetworkCommandType.Reload.ToString())
                await _permissionRepository.Get();
        }
    }
}
