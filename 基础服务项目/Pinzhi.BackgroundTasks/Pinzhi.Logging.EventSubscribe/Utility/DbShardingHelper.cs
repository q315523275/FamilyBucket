using System;

namespace Pinzhi.Logging.EventSubscribe.Utility
{
    /// <summary>
    /// 数据库分表分库帮助类
    /// </summary>
    public class DbShardingHelper
    {
        public static string DayRule(DateTime time)
        {
            return time.ToString("yyyyMMdd");
        }

        public static string MonthRule(DateTime time)
        {
            return time.ToString("yyyyMM");
        }
    }
}
