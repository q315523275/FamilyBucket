using Pinzhi.Platform.DTO;
using Pinzhi.Platform.Model;

namespace AutoMapper
{
    public class MyMapper : Profile
    {
        public MyMapper()
        {
            CreateMap<SetPlatformInput, PlatformInfo>();
            CreateMap<SetMenuInput, MenuInfo>();
            CreateMap<SetRoleInput, RoleInfo>();
            CreateMap<SetUserInput, UserInfo>();
            CreateMap<SetApiInput, ApiInfo>();
            CreateMap<SetAppInfoInput, AppInfo>();
            CreateMap<SetAppProjectInfoInput, AppNamespaceInfo>();
            CreateMap<SetAppConfigInfoInput, AppConfigInfo>();
        }
    }
}
