using Pinzhi.Platform.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IConfigBusiness
    {
        Task<QueryAppListOutput> QueryAppList();
        Task<SetAppInfoOutput> SetAppInfo(SetAppInfoInput input);

        Task<QueryAppProjectListOutput> QueryAppProjectList(QueryAppProjectListInput input);

        Task<SetAppProjectInfoOutput> SetAppProjectInfo(SetAppProjectInfoInput input);

        Task<QueryAppConfigListOutput> QueryAppConfigList(QueryAppConfigListInput input);

        Task<SetAppConfigInfoOutput> SetAppConfigInfo(SetAppConfigInfoInput input);
    }
}
