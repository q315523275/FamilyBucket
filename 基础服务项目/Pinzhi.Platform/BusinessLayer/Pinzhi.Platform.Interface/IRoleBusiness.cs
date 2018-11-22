using Pinzhi.Platform.Dto;
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
        /// 查询角色权限信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueryRoleInfoOutput> QueryRoleInfo(QueryRoleInfoInput input);
        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetRoleOutput> SetRole(SetRoleInput input);
    }
}
