using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Bucket.WebApi.Hangfire
{
    /// <summary>
	/// HangfireDashboard权限过滤器
	/// 目前需求只要可以登录坐标系就有权限进入Dashboard
	/// </summary>
	public class HangfireAuthorizationFilter : AuthorizeAttribute, IDashboardAuthorizationFilter
    {
        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize([NotNull] DashboardContext context)
        {
            // 需要单独进行token获取验证,与微服务jwt验证不匹配 或者使用组件 Hangfire.Dashboard.Authorization
            // context.GetHttpContext().User.Identity.IsAuthenticated;
            // 
            Console.WriteLine(context.AntiforgeryToken);
            return true;
        }
    }
}
