using Ocelot.Configuration.File;

namespace Pinzhi.Platform.Dto.Microservice
{
    public class SetApiGatewayReRouteInput: FileReRoute
    {
        public int Id { set; get; }
        public int GatewayId { set; get; }
        public int State { set; get; }
    }
}
