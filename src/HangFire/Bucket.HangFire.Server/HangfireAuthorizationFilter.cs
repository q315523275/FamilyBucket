using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;

namespace Bucket.HangFire.Server
{
    /// <summary>
	/// HangfireDashboard权限过滤器
	/// 目前需求只要可以登录坐标系就有权限进入Dashboard
	/// </summary>
	public class HangfireAuthorizationFilter : AuthorizeAttribute, IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
