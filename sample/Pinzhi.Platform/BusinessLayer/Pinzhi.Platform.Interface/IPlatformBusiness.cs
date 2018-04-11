using Pinzhi.Platform.DTO;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IPlatformBusiness
    {
        /// <summary>
        /// 查询平台列表
        /// </summary>
        /// <returns></returns>
        Task<QueryPlatformsOutput> QueryPlatforms();
        /// <summary>
        /// 设置平台信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetPlatformOutput> SetPlatform(SetPlatformInput input);
    }
}
