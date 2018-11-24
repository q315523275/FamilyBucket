using Microsoft.Extensions.Options;

namespace Bucket.ErrorCode
{
    public class HttpUrlRepository : IHttpUrlRepository
    {
        private ErrorCodeSetting _errorCodeConfiguration;

        public HttpUrlRepository(IOptions<ErrorCodeSetting> errorCodeConfiguration)
        {
            _errorCodeConfiguration = errorCodeConfiguration.Value;
        }

        public string GetApiUrl()
        {
            string url = _errorCodeConfiguration.ServerUrl.TrimEnd('/');
            var uri = $"{url}/ErrorCode/GetList";
            var query = $"source=PZGO";
            return $"{uri}?{query}";
        }
    }
}
