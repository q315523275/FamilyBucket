using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bucket.Config;
using Bucket.DbContext;
using Bucket.Exceptions;
using Bucket.Redis;
using Bucket.ServiceDiscovery;
using Bucket.Utility.Helpers;
using Ocelot.Configuration.File;
using Pinzhi.Platform.Business;
using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Dto.Microservice;
using Pinzhi.Platform.Model;

namespace Pinzhi.Platform.Interface
{
    /// <summary>
    /// 微服务管理业务
    /// </summary>
    public class MicroserviceBusiness : IMicroserviceBusiness
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IConfig _config;
        private readonly IDbRepository<ApiGatewayConfigurationInfo> _configDbRepository;
        private readonly IDbRepository<ApiGatewayReRouteInfo> _routeDbRepository;
        private readonly RedisClient _redisClient;
        public MicroserviceBusiness(IServiceDiscovery serviceDiscovery, 
            IConfig config, 
            IDbRepository<ApiGatewayConfigurationInfo> configDbRepository, 
            IDbRepository<ApiGatewayReRouteInfo> routeDbRepository, 
            RedisClient redisClient)
        {
            _serviceDiscovery = serviceDiscovery;
            _config = config;
            _configDbRepository = configDbRepository;
            _routeDbRepository = routeDbRepository;
            _redisClient = redisClient;
        }


        /// <summary>
        /// 查询服务发现服务列表
        /// </summary>
        /// <returns></returns>
        public async Task<QueryServiceListOutput> QueryServiceList(QueryServiceListInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                return new QueryServiceListOutput { Data = await _serviceDiscovery.FindServiceInstancesAsync() };
            }
            else
            {
                return new QueryServiceListOutput { Data = await _serviceDiscovery.FindServiceInstancesAsync(input.Name) };
            }
        }
        /// <summary>
        /// 服务注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetServiceInfoOutput> SetServiceInfo(SetServiceInfoInput input)
        {
            var result = await _serviceDiscovery.RegisterServiceAsync(serviceName: input.Name,
                version: input.Version,
                uri: new Uri($"http://{input.HostAndPort.Host}:{input.HostAndPort.Port}"),
                healthCheckUri: new Uri(input.HealthCheckUri),
                tags: input.Tags);
            return new SetServiceInfoOutput { };
        }
        /// <summary>
        /// 服务移除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DeleteServiceOutput> DeleteService(DeleteServiceInput input)
        {
            await _serviceDiscovery.DeregisterServiceAsync(input.ServiceId);
            return new DeleteServiceOutput { };
        }
        /// <summary>
        /// 查询网关配置列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryApiGatewayConfigurationOutput> QueryApiGatewayConfiguration(QueryApiGatewayConfigurationInput input)
        {
            var pageModel = new SqlSugar.PageModel { PageIndex = input.PageIndex, PageSize = input.PageSize };
            var list = await _configDbRepository.GetPageListAsync(it => true, pageModel);
            return new QueryApiGatewayConfigurationOutput { CurrentPage = pageModel.PageIndex, Total = pageModel.PageCount, Data = list };
        }
        /// <summary>
        /// 设置网关配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BaseOutput> SetApiGatewayConfiguration(SetApiGatewayConfigurationInput input)
        {
            var configInfo = new ApiGatewayConfigurationInfo
            {
                GatewayId = input.GatewayId,
                BaseUrl = input.BaseUrl,
                DownstreamScheme = input.DownstreamScheme,
                GatewayKey = input.GatewayKey,
                HttpHandlerOptions = Json.ToJson(input.HttpHandlerOptions),
                LoadBalancerOptions = Json.ToJson(input.LoadBalancerOptions),
                QoSOptions = Json.ToJson(input.QoSOptions),
                RateLimitOptions = Json.ToJson(input.RateLimitOptions),
                RequestIdKey = input.RequestIdKey,
                ServiceDiscoveryProvider = Json.ToJson(input.ServiceDiscoveryProvider)
            };
            if (configInfo.GatewayId > 0)
                await _configDbRepository.UpdateAsync(configInfo);
            else
                await _configDbRepository.InsertAsync(configInfo);
            return new BaseOutput { };
        }
        /// <summary>
        /// 查询网关路由列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryApiGatewayReRouteListOutput> QueryApiGatewayReRouteList(QueryApiGatewayReRouteListInput input)
        {
            int count = 0;
            var listResult = await _routeDbRepository.AsQueryable()
                                                      .WhereIF(input.GatewayId > 0, it => it.GatewayId == input.GatewayId)
                                                      .WhereIF(input.State > -1, it => it.State == input.State)
                                                      .ToPageListAsync(input.PageIndex, input.PageSize, count);
            return new QueryApiGatewayReRouteListOutput { CurrentPage = input.PageIndex, Total = listResult.Value, Data = listResult.Key };
        }
        /// <summary>
        /// 设置网关路由
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BaseOutput> SetApiGatewayReRoute(SetApiGatewayReRouteInput input)
        {
            var rerouteInfo = new ApiGatewayReRouteInfo
            {
                AuthenticationOptions = Json.ToJson(input.AuthenticationOptions),
                CacheOptions = Json.ToJson(input.FileCacheOptions),
                DelegatingHandlers = Json.ToJson(input.DelegatingHandlers),
                DownstreamHostAndPorts = Json.ToJson(input.DownstreamHostAndPorts),
                DownstreamPathTemplate = input.DownstreamPathTemplate,
                Id = input.Id,
                Key = input.Key,
                Priority = input.Priority,
                SecurityOptions = Json.ToJson(input.SecurityOptions),
                ServiceName = input.ServiceName,
                State = input.State,
                Timeout = input.Timeout,
                UpstreamHost = input.UpstreamHost,
                UpstreamHttpMethod = Json.ToJson(input.UpstreamHttpMethod),
                UpstreamPathTemplate = input.UpstreamPathTemplate,
                GatewayId = input.GatewayId,
                DownstreamScheme = input.DownstreamScheme,
                HttpHandlerOptions = Json.ToJson(input.HttpHandlerOptions),
                LoadBalancerOptions = Json.ToJson(input.LoadBalancerOptions),
                QoSOptions = Json.ToJson(input.QoSOptions),
                RateLimitOptions = Json.ToJson(input.RateLimitOptions),
                RequestIdKey = input.RequestIdKey,
            };
            if (rerouteInfo.GatewayId > 0)
                await _routeDbRepository.UpdateAsync(rerouteInfo);
            else
            {
                if (_routeDbRepository.IsAny(it => it.UpstreamPathTemplate == rerouteInfo.UpstreamPathTemplate && it.GatewayId == rerouteInfo.GatewayId))
                    throw new BucketException("ms_003", "上游路由规则已存在");
                await _routeDbRepository.InsertAsync(rerouteInfo);
            }
            return new BaseOutput { };
        }
        /// <summary>
        /// 同步至Consul
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BaseOutput> SyncApiGatewayConfigurationToConsul(SyncApiGatewayConfigurationInput input)
        {
            var configInfo = await _configDbRepository.GetFirstAsync(it => it.GatewayId == input.GatewayId);
            if (configInfo != null)
            {
                var data = await GetGatewayData(input.GatewayId);
                await _serviceDiscovery.KeyValuePutAsync(configInfo.GatewayKey, Json.ToJson(data));
            }
            return new BaseOutput { };
        }
        /// <summary>
        /// 同步至redis
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BaseOutput> SyncApiGatewayConfigurationToRedis(SyncApiGatewayConfigurationInput input)
        {
            var configInfo = await _configDbRepository.GetFirstAsync(it => it.GatewayId == input.GatewayId);
            if (configInfo != null)
            {
                var redis = _redisClient.GetDatabase(_config.StringGet(SysConfig.RedisConnectionKey), 11);
                var data = await GetGatewayData(input.GatewayId);
                await redis.StringSetAsync($"ApiGateway:{configInfo.GatewayKey}", Json.ToJson(data));
            }
            return new BaseOutput { };
        }
        /// <summary>
        /// 查询网关配置数据
        /// </summary>
        /// <param name="gatewayId"></param>
        /// <returns></returns>
        private async Task<FileConfiguration> GetGatewayData(int gatewayId)
        {
            var fileConfig = new FileConfiguration();
            var configInfo = await _configDbRepository.GetFirstAsync(it => it.GatewayId == gatewayId);
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
                    fgc.HttpHandlerOptions = Json.ToObject<FileHttpHandlerOptions>(configInfo.HttpHandlerOptions);
                if (!string.IsNullOrWhiteSpace(configInfo.LoadBalancerOptions))
                    fgc.LoadBalancerOptions = Json.ToObject<FileLoadBalancerOptions>(configInfo.LoadBalancerOptions);
                if (!string.IsNullOrWhiteSpace(configInfo.QoSOptions))
                    fgc.QoSOptions = Json.ToObject<FileQoSOptions>(configInfo.QoSOptions);
                if (!string.IsNullOrWhiteSpace(configInfo.RateLimitOptions))
                    fgc.RateLimitOptions = Json.ToObject<FileRateLimitOptions>(configInfo.RateLimitOptions);
                if (!string.IsNullOrWhiteSpace(configInfo.ServiceDiscoveryProvider))
                    fgc.ServiceDiscoveryProvider = Json.ToObject<FileServiceDiscoveryProvider>(configInfo.ServiceDiscoveryProvider);
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
                            m.UpstreamHttpMethod = Json.ToObject<List<string>>(model.UpstreamHttpMethod);
                        if (!string.IsNullOrWhiteSpace(model.DownstreamHostAndPorts))
                            m.DownstreamHostAndPorts = Json.ToObject<List<FileHostAndPort>>(model.DownstreamHostAndPorts);
                        if (!string.IsNullOrWhiteSpace(model.SecurityOptions))
                            m.SecurityOptions = Json.ToObject<FileSecurityOptions>(model.SecurityOptions);
                        if (!string.IsNullOrWhiteSpace(model.CacheOptions))
                            m.FileCacheOptions = Json.ToObject<FileCacheOptions>(model.CacheOptions);
                        if (!string.IsNullOrWhiteSpace(model.HttpHandlerOptions))
                            m.HttpHandlerOptions = Json.ToObject<FileHttpHandlerOptions>(model.HttpHandlerOptions);
                        if (!string.IsNullOrWhiteSpace(model.AuthenticationOptions))
                            m.AuthenticationOptions = Json.ToObject<FileAuthenticationOptions>(model.AuthenticationOptions);
                        if (!string.IsNullOrWhiteSpace(model.RateLimitOptions))
                            m.RateLimitOptions = Json.ToObject<FileRateLimitRule>(model.RateLimitOptions);
                        if (!string.IsNullOrWhiteSpace(model.LoadBalancerOptions))
                            m.LoadBalancerOptions = Json.ToObject<FileLoadBalancerOptions>(model.LoadBalancerOptions);
                        if (!string.IsNullOrWhiteSpace(model.QoSOptions))
                            m.QoSOptions = Json.ToObject<FileQoSOptions>(model.QoSOptions);
                        if (!string.IsNullOrWhiteSpace(model.DelegatingHandlers))
                            m.DelegatingHandlers = Json.ToObject<List<string>>(model.DelegatingHandlers);
                        reroutelist.Add(m);
                    }
                    fileConfig.ReRoutes = reroutelist;
                }
            }
            return fileConfig;
        }
    }
}
