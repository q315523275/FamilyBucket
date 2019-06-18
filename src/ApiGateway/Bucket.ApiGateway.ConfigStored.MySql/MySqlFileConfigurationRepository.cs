using Bucket.ApiGateway.ConfigStored.MySql.Entity;
using Bucket.DbContext.SqlSugar;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.ApiGateway.ConfigStored.MySql
{
    public class MySqlFileConfigurationRepository : IFileConfigurationRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _apiGatewayKey;

        public MySqlFileConfigurationRepository(IServiceProvider serviceProvider, string apiGatewayKey)
        {
            _serviceProvider = serviceProvider;
            _apiGatewayKey = apiGatewayKey;
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _configDbRepository = scope.ServiceProvider.GetRequiredService<ISqlSugarDbRepository<ConfigurationInfo>>();
                var _routeDbRepository = scope.ServiceProvider.GetRequiredService<ISqlSugarDbRepository<ReRouteInfo>>();

                var st = DateTime.Now;
                var fileConfig = new FileConfiguration();
                var configInfo = await _configDbRepository.GetFirstAsync(it => it.GatewayKey == _apiGatewayKey);
                if (configInfo != null)
                {
                    // config
                    var fgc = new FileGlobalConfiguration
                    {
                        BaseUrl = configInfo.BaseUrl,
                        DownstreamScheme = configInfo.DownstreamScheme,
                        RequestIdKey = configInfo.RequestIdKey,
                    };
                    if (!string.IsNullOrWhiteSpace(configInfo.HttpHandlerOptions))
                        fgc.HttpHandlerOptions = ToObject<FileHttpHandlerOptions>(configInfo.HttpHandlerOptions);
                    if (!string.IsNullOrWhiteSpace(configInfo.LoadBalancerOptions))
                        fgc.LoadBalancerOptions = ToObject<FileLoadBalancerOptions>(configInfo.LoadBalancerOptions);
                    if (!string.IsNullOrWhiteSpace(configInfo.QoSOptions))
                        fgc.QoSOptions = ToObject<FileQoSOptions>(configInfo.QoSOptions);
                    if (!string.IsNullOrWhiteSpace(configInfo.RateLimitOptions))
                        fgc.RateLimitOptions = ToObject<FileRateLimitOptions>(configInfo.RateLimitOptions);
                    if (!string.IsNullOrWhiteSpace(configInfo.ServiceDiscoveryProvider))
                        fgc.ServiceDiscoveryProvider = ToObject<FileServiceDiscoveryProvider>(configInfo.ServiceDiscoveryProvider);
                    fileConfig.GlobalConfiguration = fgc;

                    // reroutes
                    var reRouteResult = await _routeDbRepository.GetListAsync(it => it.GatewayId == configInfo.GatewayId && it.State == 1);
                    if (reRouteResult.Count > 0)
                    {
                        var reroutelist = new List<FileReRoute>();
                        foreach (var model in reRouteResult)
                        {
                            var m = new FileReRoute()
                            {
                                UpstreamHost = model.UpstreamHost,
                                UpstreamPathTemplate = model.UpstreamPathTemplate,

                                DownstreamPathTemplate = model.DownstreamPathTemplate,
                                DownstreamScheme = model.DownstreamScheme,

                                ServiceName = model.ServiceName,
                                Priority = model.Priority,
                                RequestIdKey = model.RequestIdKey,
                                Key = model.Key,
                                Timeout = model.Timeout,
                            };
                            if (!string.IsNullOrWhiteSpace(model.UpstreamHttpMethod))
                                m.UpstreamHttpMethod = ToObject<List<string>>(model.UpstreamHttpMethod);
                            if (!string.IsNullOrWhiteSpace(model.DownstreamHostAndPorts))
                                m.DownstreamHostAndPorts = ToObject<List<FileHostAndPort>>(model.DownstreamHostAndPorts);
                            if (!string.IsNullOrWhiteSpace(model.SecurityOptions))
                                m.SecurityOptions = ToObject<FileSecurityOptions>(model.SecurityOptions);
                            if (!string.IsNullOrWhiteSpace(model.CacheOptions))
                                m.FileCacheOptions = ToObject<FileCacheOptions>(model.CacheOptions);
                            if (!string.IsNullOrWhiteSpace(model.HttpHandlerOptions))
                                m.HttpHandlerOptions = ToObject<FileHttpHandlerOptions>(model.HttpHandlerOptions);
                            if (!string.IsNullOrWhiteSpace(model.AuthenticationOptions))
                                m.AuthenticationOptions = ToObject<FileAuthenticationOptions>(model.AuthenticationOptions);
                            if (!string.IsNullOrWhiteSpace(model.RateLimitOptions))
                                m.RateLimitOptions = ToObject<FileRateLimitRule>(model.RateLimitOptions);
                            if (!string.IsNullOrWhiteSpace(model.LoadBalancerOptions))
                                m.LoadBalancerOptions = ToObject<FileLoadBalancerOptions>(model.LoadBalancerOptions);
                            if (!string.IsNullOrWhiteSpace(model.QoSOptions))
                                m.QoSOptions = ToObject<FileQoSOptions>(model.QoSOptions);
                            if (!string.IsNullOrWhiteSpace(model.DelegatingHandlers))
                                m.DelegatingHandlers = ToObject<List<string>>(model.DelegatingHandlers);
                            reroutelist.Add(m);
                        }
                        fileConfig.ReRoutes = reroutelist;
                    }
                }
                // Console.WriteLine((DateTime.Now - st).TotalMilliseconds);
                return new OkResponse<FileConfiguration>(fileConfig);
            }
        }

        public async Task<Response> Set(FileConfiguration fileConfiguration)
        {
            return await Task.FromResult(new OkResponse());
        }

        /// <summary>
        /// 将Json字符串转换为对象
        /// </summary>
        /// <param name="json">Json字符串</param>
        private T ToObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
