using SqlSugar;

namespace Bucket.ApiGateway.ConfigStored.MySql.Entity
{
    [SugarTable("tb_apigateway_reroute")]
    public class ReRouteInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { set; get; }
        public int GatewayId { set; get; }
        public string UpstreamPathTemplate { set; get; }
        public string UpstreamHttpMethod { set; get; }
        public string UpstreamHost { set; get; }
        public string DownstreamPathTemplate { set; get; }
        public string DownstreamScheme { set; get; }
        public string DownstreamHostAndPorts { set; get; }
        public string ServiceName { set; get; }
        public string Key { set; get; }
        public string RequestIdKey { set; get; }
        public int Priority { set; get; }
        public int Timeout { set; get; }
        public string SecurityOptions { set; get; }
        public string CacheOptions { set; get; }
        public string HttpHandlerOptions { set; get; }
        public string AuthenticationOptions { set; get; }
        public string RateLimitOptions { set; get; }
        public string LoadBalancerOptions { set; get; }
        public string QoSOptions { set; get; }
        public string DelegatingHandlers { set; get; }
        public int State { set; get; }
    }
}
