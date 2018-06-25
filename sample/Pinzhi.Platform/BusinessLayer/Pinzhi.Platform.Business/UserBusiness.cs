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
using Bucket.Utility.Helpers;

namespace Pinzhi.Platform.Business
{
    public class UserBusiness: IUserBusiness
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly SqlSugarClient _dbContext;
        private readonly RedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        private readonly IRoleBusiness _roleBusiness;
        public UserBusiness(SqlSugarClient dbContext,
            IMapper mapper,
            RedisClient redisClient,
            IJsonHelper jsonHelper,
            IUser user,
            IRoleBusiness roleBusiness)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _user = user;
            _roleBusiness = roleBusiness;
        }
        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryUsersOutput> QueryUsers(QueryUsersInput input)
        {
            var list = new List<QueryUserDTO>();
            var result = new QueryUsersOutput { Data = new List<QueryUserDTO>() };
            var totalNumber = 0;
            if (input.RoleId > 0)
            {
                var query = await _dbContext.Queryable<UserInfo, UserRoleInfo>((u, urole) => new object[] { JoinType.Inner, u.Id == urole.Uid })
                .Where((u, urole) => urole.RoleId == input.RoleId)
                .WhereIF(input.State > -1, (u, urole) => u.State == input.State)
                .WhereIF(!input.RealName.IsEmpty(), (u, urole) => u.RealName == input.RealName)
                .WhereIF(!input.UserName.IsEmpty(), (u, urole) => u.UserName == input.UserName)
                .WhereIF(!input.Mobile.IsEmpty(), (u, urole) => u.Mobile == input.Mobile)
                .Select((u, urole) => new QueryUserDTO { Id = u.Id, Mobile = u.Mobile, RealName = u.RealName, State = u.State, UpdateTime = u.UpdateTime, UserName = u.UserName, Email = u.Email })
                .ToPageListAsync(input.PageIndex, input.PageSize, totalNumber);
                list = query.Key;
                totalNumber = query.Value;
            }
            else if (!input.ProjectName.IsEmpty())
            {
                // 项目角色Id数组
                var prlist = _dbContext.Queryable<RoleInfo>().Where(it => it.ProjectName == input.ProjectName && it.IsDel == false).Select(it => new { it.Id }).ToList();
                var roleIdArr = prlist.Select(it => it.Id).ToArray();
                // 查询
                var query = await _dbContext.Queryable<UserInfo, UserRoleInfo>((u, urole) => new object[] { JoinType.Inner, u.Id == urole.Uid })
                .Where((u, urole) => roleIdArr.Contains(urole.RoleId))
                .WhereIF(input.State > -1, (u, urole) => u.State == input.State)
                .WhereIF(!input.RealName.IsEmpty(), (u, urole) => u.RealName == input.RealName)
                .WhereIF(!input.UserName.IsEmpty(), (u, urole) => u.UserName == input.UserName)
                .WhereIF(!input.Mobile.IsEmpty(), (u, urole) => u.Mobile == input.Mobile)
                .GroupBy((u, urole) => u.Id)
                .Select((u, urole) => new QueryUserDTO { Id = u.Id, Mobile = u.Mobile, RealName = u.RealName, State = u.State, UpdateTime = u.UpdateTime, UserName = u.UserName, Email = u.Email })
                .ToPageListAsync(input.PageIndex, input.PageSize, totalNumber);
                list = query.Key;
                totalNumber = query.Value;
            }
            else
            {
                var query = await _dbContext.Queryable<UserInfo>()
                .WhereIF(input.State > -1, f => f.State == input.State)
                .WhereIF(!input.RealName.IsEmpty(), f => f.RealName == input.RealName)
                .WhereIF(!input.UserName.IsEmpty(), f => f.UserName == input.UserName)
                .WhereIF(!input.Mobile.IsEmpty(), f => f.Mobile == input.Mobile)
                .Select(u => new QueryUserDTO { Id = u.Id, Mobile = u.Mobile, RealName = u.RealName, State = u.State, UpdateTime = u.UpdateTime, UserName = u.UserName, Email = u.Email })
                .ToPageListAsync(input.PageIndex, input.PageSize, totalNumber);
                list = query.Key;
                totalNumber = query.Value;
            }
            var canUseRoleList = await _roleBusiness.QueryRoles(new QueryRolesInput());
            var canUseRole = canUseRoleList.Data as List<RoleInfo>;
            result.CurrentPage = input.PageIndex;
            result.Total = totalNumber;
            result.Data = list;
            result.Data.ForEach(m =>
            {
                var useRole = _dbContext.Queryable<UserRoleInfo>()
                             .Where(it => it.Uid == m.Id)
                             .Select(it => new { Id = it.RoleId })
                             .ToList();
                var idList = useRole.GroupBy(p => p.Id).Select(it => it.First().Id).ToList();
                m.RoleList = canUseRole.Where(it => idList.Contains(it.Id)).Select(it => new { Id = it.Id, ProjectName = it.ProjectName, Name = it.Name }).ToList();
            });
            return result;
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetUserOutput> SetUser(SetUserInput input)
        {         
            var model = _mapper.Map<SetUserInput, UserInfo>(input);
            if (model.Id > 0)
            {
                model.UpdateTime = DateTime.Now;
                if (!model.Password.IsEmpty())
                {
                    model.Salt = Randoms.CreateRandomValue(8, false);
                    model.Password = Encrypt.SHA256(model.Password + model.Salt);
                    // 基础字段不容许更新
                    await _dbContext.Updateable(model)
                                    .IgnoreColumns(it => new { it.UserName, it.Mobile, it.CreateTime })
                                    .ExecuteCommandAsync();
                }
                else
                {
                    // 基础字段不容许更新
                    await _dbContext.Updateable(model)
                                    .IgnoreColumns(it => new { it.UserName, it.Password, it.Salt, it.Mobile, it.CreateTime })
                                    .ExecuteCommandAsync();
                }
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.Salt = Randoms.CreateRandomValue(8, false);
                model.Password = Encrypt.SHA256(model.Password + model.Salt);
                model.Id = Convert.ToInt64($"{Time.GetUnixTimestamp()}{ Randoms.CreateRandomValue(3, true) }");
                await _dbContext.Insertable(model).ExecuteCommandAsync();
            }
            // 用户角色操作
            List<UserRoleInfo> userRoleList = new List<UserRoleInfo>();
            foreach (var id in input.RoleIdList)
            {
                // 防止重复数据
                if (!userRoleList.Exists(it => it.RoleId == id))
                {
                    userRoleList.Add(new UserRoleInfo
                    {
                        Uid = model.Id,
                        RoleId = id
                    });
                }
            }
            // 删除用户当前角色
            await _dbContext.Deleteable<UserRoleInfo>().Where(f => f.Uid == model.Id).ExecuteCommandAsync();
            // 添加用户角色
            if (userRoleList.Count > 0)
                await _dbContext.Insertable(userRoleList).ExecuteCommandAsync();

            return new SetUserOutput { };
        }
    }
}
