using Pinzhi.Platform.DTO;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IMenuBusiness
    {
        /// <summary>
        /// 查询平台菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueryAllMenusOutput> QueryAllMenus(QueryAllMenusInput input);
        /// <summary>
        /// 设置平台菜单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetMenuOutput> SetPlatform(SetMenuInput input);
        /// <summary>
        /// 查询登陆用户拥有菜单
        /// </summary>
        /// <returns></returns>
        Task<QueryUserMenusOutput> QueryUserMenus();
    }
}
