using AutoMapper;
using Bucket.ConfigCenter;
using Bucket.Core;
using Bucket.Redis;
using Pinzhi.Platform.DTO;
using Pinzhi.Platform.Interface;
using Pinzhi.Platform.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Business
{
    /// <summary>
    /// 平台管理业务类
    /// </summary>
    public class PlatformBusiness: IPlatformBusiness
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly SqlSugarClient _dbContext;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IConfigCenter _configCenter;
        private readonly RedisClient _redisClient;
        public PlatformBusiness(SqlSugarClient dbContext, IMapper mapper,RedisClient redisClient,IJsonHelper jsonHelper, IConfigCenter configCenter)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _configCenter = configCenter;
        }
        /// <summary>
        /// 查询平台列表
        /// </summary>
        /// <returns></returns>
        public async Task<QueryPlatformsOutput> QueryPlatforms()
        {
            var redis = _redisClient.GetDatabase(_configCenter.Get(SysConfig.RedisConnectionKey, "localhost:6379,allowadmin=true"), 2);
            var redisList = await redis.StringGetAsync(CacheKeys.PlatformKey);
            if (!string.IsNullOrWhiteSpace(redisList))
            {
                return new QueryPlatformsOutput { Data = _jsonHelper.DeserializeObject<List<PlatformInfo>>(redisList) };
            }
            else
            {
                var list = await _dbContext.Queryable<PlatformInfo>().OrderBy(it => it.SortId, OrderByType.Asc).ToListAsync();
                await redis.StringSetAsync(CacheKeys.PlatformKey, _jsonHelper.SerializeObject(list));
                return new QueryPlatformsOutput { Data = list };
            }
        }
        /// <summary>
        /// 设置平台信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetPlatformOutput> SetPlatform(SetPlatformInput input)
        {
            var model = _mapper.Map<PlatformInfo>(input);
            if (model.Id > 0)
            {
                await _dbContext.Updateable(model).ExecuteCommandAsync();
            }
            else
            {
                model.AddTime = DateTime.Now;
                model.IsDel = false;
                await _dbContext.Insertable(model).ExecuteCommandAsync();
            }

            var redis = _redisClient.GetDatabase(_configCenter.Get(SysConfig.RedisConnectionKey, "192.168.1.199:6379,allowadmin=true"), 2);
            await redis.KeyDeleteAsync(CacheKeys.PlatformKey);

            return new SetPlatformOutput { };
        }
    }
}
