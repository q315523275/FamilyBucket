using Bucket.Config;
using Bucket.Exceptions;
using Bucket.Utility;
using Newtonsoft.Json.Linq;
using Pinzhi.Identity.Dto.Wx;
using Pinzhi.Identity.Interface;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pinzhi.Identity.Repository
{
    public class WxRepository: IWxRepository
    {
        private readonly IConfig _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public WxRepository(IConfig config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 查询微信小程序openid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<QueryOpenIdOutput> QueryOpenIdAsync(string code, string appid)
        {
            var apibaseurl = _config.StringGet("WxMiniApiUrl");
            var appsecret = _config.StringGet($"WxMiniAppSecret_{appid}");
            var wxapiurl = $"{apibaseurl}/sns/jscode2session?appid={appid}&secret={appsecret}&js_code={code}&grant_type=authorization_code";
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(new HttpRequestMessage { RequestUri = new Uri(wxapiurl), Method = HttpMethod.Get });
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jobject = JObject.Parse(await response.Content.ReadAsStringAsync());
                if (jobject.Property("errcode") != null)
                    throw new BucketException($"wxmini_{jobject.GetValue("errcode")}", "微信小程序code无效");
                var output = new QueryOpenIdOutput();
                if (jobject.Property("openid") != null)
                    output.OpenId = jobject.GetValue("openid").SafeString();
                if (jobject.Property("session_key") != null)
                    output.SessionKey = jobject.GetValue("session_key").SafeString();
                if (jobject.Property("unionid") != null)
                    output.UnionId = jobject.GetValue("unionid").SafeString();
                return output;
            }
            throw new NotImplementedException();
        }
    }
}
