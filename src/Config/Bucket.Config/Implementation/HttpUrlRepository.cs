using Bucket.Config.Utils;
using Bucket.LoadBalancer;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Bucket.Config.Abstractions;

namespace Bucket.Config.Implementation
{
    public class HttpUrlRepository : IHttpUrlRepository
    {
        private readonly BucketConfigOptions _setting;
        public HttpUrlRepository(IOptions<BucketConfigOptions> setting)
        {
            _setting = setting.Value;
        }

        public string GetApiUrl(long version)
        {
            string appId = _setting.AppId;
            string secret = _setting.AppSercet;

            var path = $"/configs/{_setting.AppId}/{_setting.NamespaceName}";

            var query = $"version={version}";

            var sign = $"appId={appId}&appSecret={secret}&namespaceName={_setting.NamespaceName}";

            var pathAndQuery = $"{path}?{query}&env={_setting.Env}&sign=" + SecureHelper.SHA256(sign);

            return $"{_setting.ServerUrl.TrimEnd('/')}{pathAndQuery}";
        }
    }
}
