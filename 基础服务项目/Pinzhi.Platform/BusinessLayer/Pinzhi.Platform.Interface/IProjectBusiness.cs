using Pinzhi.Platform.Dto.Project;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IProjectBusiness
    {
        /// <summary>
        /// 查看项目列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueryProjectOutput> QueryProject();
        /// <summary>
        /// 设置项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetProjectOutput> SetProject(SetProjectInput input);
        /// <summary>
        /// 推送项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PublishCommandOutput> PublishCommand(PublishCommandInput input);
    }
}
