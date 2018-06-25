using Pinzhi.Platform.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IMicroserviceBusiness
    {
        /// <summary>
        /// 查询服务发现列表
        /// </summary>
        /// <returns></returns>
        Task<QueryServiceListOutput> QueryServiceList();
    }
}
