using AutoMapper;
using Bucket.Config;
using Bucket.Core;
using Bucket.DbContext;
using Bucket.Redis;
using Pinzhi.Platform.Dto;
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
        private readonly BucketSqlSugarClient _dbContext;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IConfig _configCenter;
        private readonly RedisClient _redisClient;
        public PlatformBusiness(BucketSqlSugarClient dbContext, IMapper mapper,RedisClient redisClient,IJsonHelper jsonHelper, IConfig configCenter)
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
            var redis = _redisClient.GetDatabase(_configCenter.StringGet(SysConfig.RedisConnectionKey), 2);
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
            try
            {
                _dbContext.Ado.BeginTran();

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

                var redis = _redisClient.GetDatabase(_configCenter.StringGet(SysConfig.RedisConnectionKey), 2);
                await redis.KeyDeleteAsync(CacheKeys.PlatformKey);

                _dbContext.Ado.CommitTran();

            }
            catch(Exception ex)
            {
                _dbContext.Ado.RollbackTran();
                throw new Exception("事务执行失败", ex);
            }
            return new SetPlatformOutput { };
        }
    }
}
