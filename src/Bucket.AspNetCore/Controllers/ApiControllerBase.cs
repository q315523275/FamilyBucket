using Bucket.AspNetCore.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Bucket.WebApi
{
    public class ApiControllerBase : Controller
    {
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        protected virtual IActionResult Success(dynamic data = null, string message = null)
        {
            if (message == null)
                message = "操作成功";
            return new Result(StateCode.Ok, message, data);
        }

        /// <summary>
        /// 返回失败消息
        /// </summary>
        /// <param name="message">消息</param>
        protected IActionResult Fail(string message)
        {
            return new Result(StateCode.Fail, message);
        }
    }
}
