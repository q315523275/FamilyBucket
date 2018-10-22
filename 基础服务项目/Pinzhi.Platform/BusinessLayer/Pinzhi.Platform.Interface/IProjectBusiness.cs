using Pinzhi.Platform.DTO;
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
    }
}
