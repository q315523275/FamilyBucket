using AutoMapper;
using Bucket.Core;
using Bucket.Redis;
using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Model;
using SqlSugar;
using System;
using System.Threading.Tasks;
using Pinzhi.Platform.Interface;
using Bucket.Utility;
using System.Collections.Generic;
using Bucket.Config;
using Bucket.DbContext;

namespace Pinzhi.Platform.Business
{
    /// <summary>
    /// 角色管理类
    /// </summary>
    public class RoleBusiness: IRoleBusiness
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly BucketSqlSugarClient _dbContext;
        private readonly RedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        private readonly IConfig _configCenter;
        public RoleBusiness(BucketSqlSugarClient dbContext,
            IMapper mapper,
            RedisClient redisClient,
            IJsonHelper jsonHelper,
            IUser user,
            IConfig configCenter)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _user = user;
            _configCenter = configCenter;
        }
        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryRolesOutput> QueryAllRoles(QueryRolesInput input)
        {
            var list = await _dbContext.Queryable<RoleInfo>()
                                 .WhereIF(!input.PlatformKey.IsEmpty(), it => it.PlatformKey == input.PlatformKey)
                                 .ToListAsync();
            return new QueryRolesOutput { Data = list };
        }
        /// <summary>
        /// 查询可用角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryRolesOutput> QueryRoles(QueryRolesInput input)
        {
            var list = new List<RoleInfo>();
            var redis = _redisClient.GetDatabase(_configCenter.StringGet(SysConfig.RedisConnectionKey), 2);
            if (string.IsNullOrWhiteSpace(input.PlatformKey))
            {
                // 查询所有角色
                var redisList = await redis.StringGetAsync(CacheKeys.RoleAllUseKey);
                if (!string.IsNullOrWhiteSpace(redisList))
                {
                    return new QueryRolesOutput { Data = _jsonHelper.DeserializeObject<List<RoleInfo>>(redisList) };
                }
                else
                {
                    list = await _dbContext.Queryable<RoleInfo>()
                                 .Where(it => it.IsDel == false)
                                 .WhereIF(!input.PlatformKey.IsEmpty(), it => it.PlatformKey == input.PlatformKey)
                                 .ToListAsync();
                    await redis.StringSetAsync(CacheKeys.RoleAllUseKey, _jsonHelper.SerializeObject(list));
                }
            }
            else
            {
                // 查询项目对应角色
                list = await _dbContext.Queryable<RoleInfo>()
                                 .Where(it => it.IsDel == false)
                                 .WhereIF(!input.PlatformKey.IsEmpty(), it => it.PlatformKey == input.PlatformKey)
                                 .ToListAsync();
            }
            return new QueryRolesOutput { Data = list };
        }

        /// <summary>
        /// 查询角色权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryRoleInfoOutput> QueryRoleInfo(QueryRoleInfoInput input)
        {
            var model = await _dbContext.Queryable<RoleInfo>().Where(it => it.Id == input.RoleId).FirstAsync();
            var apiList = await _dbContext.Queryable<RoleApiInfo>().Where(it => it.RoleId == input.RoleId).ToListAsync();
            var menuList = await _dbContext.Queryable<RoleMenuInfo>().Where(it => it.RoleId == input.RoleId).ToListAsync();
            return new QueryRoleInfoOutput { Data = new { RoleInfo = model, MenuList = menuList, ApiList = apiList } };
        }

        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetRoleOutput> SetRole(SetRoleInput input)
        {
            try
            {
                _dbContext.Ado.BeginTran();

                #region 基础信息更新
                var redis = _redisClient.GetDatabase(_configCenter.StringGet(SysConfig.RedisConnectionKey), 2);
                var model = _mapper.Map<RoleInfo>(input);
                if (model.Id > 0)
                {
                    model.UpdateTime = DateTime.Now;
                    await _dbContext.Updateable(model)
                                    .IgnoreColumns(it => new { it.PlatformKey, it.CreateTime, it.IsSys })
                                    .ExecuteCommandAsync();
                }
                else
                {
                    model.CreateTime = DateTime.Now;
                    model.IsDel = false;
                    model.Id = await _dbContext.Insertable(model)
                                               .ExecuteReturnIdentityAsync();
                }
                #endregion

                #region 菜单权限
                // 用户角色操作
                List<RoleMenuInfo> roleMenuList = new List<RoleMenuInfo>();
                foreach (var id in input.MenuIdList)
                {
                    // 防止重复数据
                    if (!roleMenuList.Exists(it => it.MenuId == id))
                    {
                        roleMenuList.Add(new RoleMenuInfo
                        {
                            MenuId = id,
                            RoleId = model.Id
                        });
                    }
                }
                // 删除用户当前角色
                await _dbContext.Deleteable<RoleMenuInfo>().Where(f => f.RoleId == model.Id).ExecuteCommandAsync();
                // 添加用户角色
                if (roleMenuList.Count > 0)
                    await _dbContext.Insertable(roleMenuList).ExecuteCommandAsync();
                #endregion

                #region 菜单权限
                // 用户角色操作
                List<RoleApiInfo> roleApiList = new List<RoleApiInfo>();
                foreach (var id in input.ApiIdList)
                {
                    // 防止重复数据
                    if (!roleApiList.Exists(it => it.ApiId == id))
                    {
                        roleApiList.Add(new RoleApiInfo
                        {
                            ApiId = id,
                            RoleId = model.Id
                        });
                    }
                }
                // 删除用户当前角色
                await _dbContext.Deleteable<RoleApiInfo>().Where(f => f.RoleId == model.Id).ExecuteCommandAsync();
                // 添加用户角色
                if (roleApiList.Count > 0)
                    await _dbContext.Insertable(roleApiList).ExecuteCommandAsync();
                #endregion

                #region 缓存更新
                await redis.KeyDeleteAsync(CacheKeys.RoleAllUseKey);
                // 应该立即更新缓存
                #endregion

                _dbContext.Ado.CommitTran();
            }
            catch(Exception ex)
            {
                _dbContext.Ado.RollbackTran();
                throw new Exception("事务执行失败", ex);
            }
            return new SetRoleOutput { };
        }
    }
}
