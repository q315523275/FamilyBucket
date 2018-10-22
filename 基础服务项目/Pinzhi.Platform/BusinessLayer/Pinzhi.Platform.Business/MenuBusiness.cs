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

namespace Pinzhi.Platform.Business
{
    /// <summary>
    /// 菜单管理业务类
    /// </summary>
    public class MenuBusiness: IMenuBusiness
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly SqlSugarClient _dbContext;
        private readonly RedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        public MenuBusiness(SqlSugarClient dbContext, 
            IMapper mapper, 
            RedisClient redisClient, 
            IJsonHelper jsonHelper,
            IUser user)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _user = user;
        }
        /// <summary>
        /// 查询平台菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryAllMenusOutput> QueryAllMenus(QueryAllMenusInput input)
        {
            var list = await _dbContext.Queryable<MenuInfo>()
                                 .WhereIF(input.PlatformId > 0, it => it.PlatformId == input.PlatformId)
                                 .ToListAsync();
            return new QueryAllMenusOutput { Data = list };
        }

        /// <summary>
        /// 设置平台菜单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetMenuOutput> SetPlatform(SetMenuInput input)
        {
            var model = _mapper.Map<MenuInfo>(input);
            if (model.Id > 0)
            {
                await _dbContext.Updateable(model).ExecuteCommandAsync();
            }
            else
            {
                await _dbContext.Insertable(model).ExecuteCommandAsync();
            }

            return new SetMenuOutput { };
        }

        /// <summary>
        /// 查询登陆用户拥有菜单
        /// </summary>
        /// <returns></returns>
        public async Task<QueryUserMenusOutput> QueryUserMenus()
        {
            var currentUid = Convert.ToInt64(_user.Id);

            var list = await _dbContext.Queryable<MenuInfo, RoleMenuInfo, UserRoleInfo>((t1, t2, t3) => new object[] {
                       JoinType.Inner, t1.Id == t2.MenuId,
                       JoinType.Inner , t2.RoleId == t3.RoleId
                       })
                       .Where((t1, t2, t3) => t1.State == 1 && t3.Uid == currentUid)
                       .OrderBy((t1, t2, t3) => t1.SortId, OrderByType.Asc)
                       .GroupBy((t1, t2, t3) => t1.Id)
                       .Select((t1, t2, t3) => new MenuInfo { Icon = t1.Icon, Id = t1.Id, LinkUrl = t1.LinkUrl, Name = t1.Name, ParentId = t1.ParentId, PlatformId = t1.PlatformId, SortId = t1.SortId, State = t1.State })
                       .ToListAsync();

            var platformIdList = list.GroupBy(p => p.PlatformId).Select(it => it.First().PlatformId).ToList();
            var pidArr = platformIdList.ToArray();
            var platformList = await _dbContext.Queryable<PlatformInfo>().Where(it=> it.IsDel == false && pidArr.Contains(it.Id)).ToListAsync();

            return new QueryUserMenusOutput { Data = new { Menu = list, Platform = platformList } };
        }
    }
}
