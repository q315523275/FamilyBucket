using Bucket.Config.Abstractions;
using Bucket.Config.Utils;
using Microsoft.Extensions.Options;
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

            var path = $"/configs/v2/{_setting.AppId}/{_setting.NamespaceName}";

            var query = $"version={version}&env={_setting.Env}";

            var sign = $"{query}&appId={appId}&appSecret={secret}&namespaceName={_setting.NamespaceName}";

            var pathAndQuery = $"{path}?{query}&sign=" + SecureHelper.SHA256(sign);

            return $"{_setting.ServerUrl.TrimEnd('/')}{pathAndQuery}";
        }
    }
}
