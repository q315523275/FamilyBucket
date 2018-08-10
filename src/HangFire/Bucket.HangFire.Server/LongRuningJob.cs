using System;
using Hangfire;
using Hangfire.Console;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
namespace Bucket.HangFire.Server
{
    [AutomaticRetry(Attempts = 0)]
    public class LongRuningJob: IRecurringJob
    {
        /// <summary>
        /// 这个log是程序日志的logger
        /// </summary>
        private static bool IsRunning;
        private static object _obj = new object();

        public void Execute(PerformContext context)
        {
            if (IsRunning == false)
            {
                lock (_obj)
                {
                    if (IsRunning == false)
                    {
                        IsRunning = true;
                        context.WriteLine("开始执行缓存更新任务");
                        try
                        {
                            context.GetJobData();

                            context.WriteLine("结束执行缓存更新任务");
                        }
                        catch (Exception ex)
                        {
                            context.WriteLine($"执行缓存更新任务出现异常:{ex}");
                        }
                        IsRunning = false;
                    }
                }
            }
        }
    }
}
