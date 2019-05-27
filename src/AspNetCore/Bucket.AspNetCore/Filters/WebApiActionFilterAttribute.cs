using Newtonsoft.Json;
using Bucket.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
namespace Bucket.AspNetCore.Filters
{
    public class WebApiActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            #region 自定义模型验证
            if (!context.ModelState.IsValid)
            {
                var message = string.Empty;
                //获取第一个错误提示
                foreach (var key in context.ModelState.Keys)
                {
                    var state = context.ModelState[key];
                    if (state.Errors.Any())
                    {
                        message = state.Errors.First().ErrorMessage;
                        break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var errorInfo = JsonConvert.DeserializeObject<ErrorResult>(message);
                    if (errorInfo != null)
                        throw new BucketException(errorInfo.ErrorCode, errorInfo.Message);
                    else
                        throw new BucketException("-1", message);
                }
            }
            #endregion

        }
    }
}
