using Pinzhi.Platform.DTO;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IRoleBusiness
    {
        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueryRolesOutput> QueryAllRoles(QueryRolesInput input);
        /// <summary>
        /// 查询可用角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueryRolesOutput> QueryRoles(QueryRolesInput input);
        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetRoleOutput> SetRole(SetRoleInput input);
        /// <summary>
        /// 设置角色菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetRoleMenuOutput> SetRoleMenu(SetRoleMenuInput input);
        /// <summary>
        /// 设置角色接口权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetRoleApiOutput> SetRoleApi(SetRoleApiInput input);
    }
}
