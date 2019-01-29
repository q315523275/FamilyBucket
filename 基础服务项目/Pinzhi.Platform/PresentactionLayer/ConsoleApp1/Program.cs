using System;
using System.Text;
using Bucket.Utility.Helpers;
using Consul;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Pinzhi.Platform.Model;
using SqlSugar;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ConsulClient _consul = new ConsulClient(config =>
            {
                config.Address = new Uri("http://192.168.1.52:8500");
            });
            using (var db = new SqlSugarClient(new ConnectionConfig { ConnectionString = "server=192.168.1.168;port=3306;database=pzsupper;uid=imoral;pwd=imoral#@!123;", DbType = DbType.MySql, InitKeyType = InitKeyType.Attribute, IsAutoCloseConnection = false }))
            {

                var queryResult = _consul.KV.Get("ApiGatewayConfiguration").GetAwaiter().GetResult();
                var fileConfig = JsonConvert.DeserializeObject<FileConfiguration>(Encoding.UTF8.GetString(queryResult.Response.Value));

                var configInfo = new ApiGatewayConfigurationInfo
                {
                    BaseUrl = fileConfig.GlobalConfiguration.BaseUrl,
                    DownstreamScheme = fileConfig.GlobalConfiguration.DownstreamScheme,
                    GatewayKey = "Pinzhi.ApiGateway",
                    HttpHandlerOptions = Json.ToJson(fileConfig.GlobalConfiguration.HttpHandlerOptions),
                    LoadBalancerOptions = Json.ToJson(fileConfig.GlobalConfiguration.LoadBalancerOptions),
                    QoSOptions = Json.ToJson(fileConfig.GlobalConfiguration.QoSOptions),
                    RateLimitOptions = Json.ToJson(fileConfig.GlobalConfiguration.RateLimitOptions),
                    RequestIdKey = fileConfig.GlobalConfiguration.RequestIdKey,
                    ServiceDiscoveryProvider = Json.ToJson(fileConfig.GlobalConfiguration.ServiceDiscoveryProvider)
                };
                db.Insertable(configInfo).ExecuteCommand();
                foreach (var input in fileConfig.ReRoutes)
                {
                    var rerouteInfo = new ApiGatewayReRouteInfo
                    {
                        AuthenticationOptions = Json.ToJson(input.AuthenticationOptions),
                        CacheOptions = Json.ToJson(input.FileCacheOptions),
                        DelegatingHandlers = Json.ToJson(input.DelegatingHandlers),
                        DownstreamHostAndPorts = Json.ToJson(input.DownstreamHostAndPorts),
                        DownstreamPathTemplate = input.DownstreamPathTemplate,
                        Key = input.Key,
                        Priority = input.Priority,
                        SecurityOptions = Json.ToJson(input.SecurityOptions),
                        ServiceName = input.ServiceName,
                        State = 1,
                        Timeout = input.Timeout,
                        UpstreamHost = input.UpstreamHost,
                        UpstreamHttpMethod = Json.ToJson(input.UpstreamHttpMethod),
                        UpstreamPathTemplate = input.UpstreamPathTemplate,
                        GatewayId = 1,
                        DownstreamScheme = input.DownstreamScheme,
                        HttpHandlerOptions = Json.ToJson(input.HttpHandlerOptions),
                        LoadBalancerOptions = Json.ToJson(input.LoadBalancerOptions),
                        QoSOptions = Json.ToJson(input.QoSOptions),
                        RateLimitOptions = Json.ToJson(input.RateLimitOptions),
                        RequestIdKey = input.RequestIdKey,
                    };
                    db.Insertable(rerouteInfo).ExecuteCommand();
                }
            }
            Console.WriteLine("完成");
            Console.ReadLine();
        }
    }
}
