using AutoMapper;
using Bucket.Core;
using Bucket.Redis;
using Pinzhi.Platform.DTO;
using Pinzhi.Platform.Model;
using SqlSugar;
using System.Linq;
using System;
using System.Threading.Tasks;
using Pinzhi.Platform.Interface;
using Bucket.Utility;
using System.Collections.Generic;
using Bucket.ConfigCenter;
using Bucket.Utility;
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
        private readonly SqlSugarClient _dbContext;
        private readonly RedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        private readonly IConfigCenter _configCenter;
        public RoleBusiness(SqlSugarClient dbContext,
            IMapper mapper,
            RedisClient redisClient,
            IJsonHelper jsonHelper,
            IUser user,
            IConfigCenter configCenter)
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
                                 .WhereIF(!input.ProjectKey.IsEmpty(), it => it.ProjectName == input.ProjectKey)
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
            var redis = _redisClient.GetDatabase(_configCenter.Get(SysConfig.RedisConnectionKey, "192.168.1.199:6379,allowadmin=true"), 2);
            if (string.IsNullOrWhiteSpace(input.ProjectKey))
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
                                 .WhereIF(!input.ProjectKey.IsEmpty(), it => it.ProjectName == input.ProjectKey)
                                 .ToListAsync();
                    await redis.StringSetAsync(CacheKeys.RoleAllUseKey, _jsonHelper.SerializeObject(list));
                }
            }
            else
            {
                // 查询项目对应角色
                list = await _dbContext.Queryable<RoleInfo>()
                                 .Where(it => it.IsDel == false)
                                 .WhereIF(!input.ProjectKey.IsEmpty(), it => it.ProjectName == input.ProjectKey)
                                 .ToListAsync();
            }
            return new QueryRolesOutput { Data = list };
        }
        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetRoleOutput> SetRole(SetRoleInput input)
        {
            var redis = _redisClient.GetDatabase(_configCenter.Get(SysConfig.RedisConnectionKey, "192.168.1.199:6379,allowadmin=true"), 2);
            var model = _mapper.Map<RoleInfo>(input);
            if (model.Id > 0)
            {
                model.UpdateTime = DateTime.Now;
                await _dbContext.Updateable(model).IgnoreColumns(it=> new { it.CreateTime, it.IsSys }).ExecuteCommandAsync();
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.IsDel = false;
                await _dbContext.Insertable(model).ExecuteCommandAsync();
            }
            await redis.KeyDeleteAsync(CacheKeys.RoleAllUseKey);
            return new SetRoleOutput { };
        }
        /// <summary>
        /// 设置角色菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetRoleMenuOutput> SetRoleMenu(SetRoleMenuInput input)
        {
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
                        RoleId = input.RoleId
                    });
                }
            }
            // 删除用户当前角色
            await _dbContext.Deleteable<RoleMenuInfo>().Where(f => f.RoleId == input.RoleId).ExecuteCommandAsync();
            // 添加用户角色
            if (roleMenuList.Count > 0)
                await _dbContext.Insertable(roleMenuList).ExecuteCommandAsync();

            return new SetRoleMenuOutput { };
        }
        /// <summary>
        /// 设置角色接口权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetRoleApiOutput> SetRoleApi(SetRoleApiInput input)
        {
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
                        RoleId = input.RoleId
                    });
                }
            }
            // 删除用户当前角色
            await _dbContext.Deleteable<RoleApiInfo>().Where(f => f.RoleId == input.RoleId).ExecuteCommandAsync();
            // 添加用户角色
            if (roleApiList.Count > 0)
                await _dbContext.Insertable(roleApiList).ExecuteCommandAsync();

            return new SetRoleApiOutput { };
        }
    }
}
