using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Dto.Microservice;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IMicroserviceBusiness
    {
        /// <summary>
        /// 查询服务发现列表
        /// </summary>
        /// <returns></returns>
        Task<QueryServiceListOutput> QueryServiceList(QueryServiceListInput input);
        Task<SetServiceInfoOutput> SetServiceInfo(SetServiceInfoInput input);
        Task<DeleteServiceOutput> DeleteService(DeleteServiceInput input);
        Task<QueryApiGatewayConfigurationOutput> QueryApiGatewayConfiguration(QueryApiGatewayConfigurationInput input);
        Task<BaseOutput> SetApiGatewayConfiguration(SetApiGatewayConfigurationInput input);
        Task<QueryApiGatewayReRouteListOutput> QueryApiGatewayReRouteList(QueryApiGatewayReRouteListInput input);
        Task<BaseOutput> SetApiGatewayReRoute(SetApiGatewayReRouteInput input);
        Task<BaseOutput> SyncApiGatewayConfigurationToConsul(SyncApiGatewayConfigurationInput input);
        Task<BaseOutput> SyncApiGatewayConfigurationToRedis(SyncApiGatewayConfigurationInput input);
    }
}
