using Pinzhi.Platform.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IApiBusiness
    {
        /// <summary>
        /// 查询Api资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueryApisOutput> QueryApis(QueryApisInput input);
        /// <summary>
        /// 设置Api资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetApiOutput> SetApi(SetApiInput input);
    }
}
