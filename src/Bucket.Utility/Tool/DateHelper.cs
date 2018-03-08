using System;

namespace Bucket.Tool
{
    public class DateHelper
    {
        /// <summary>
        /// 日期转换成unix时间戳,13位
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime.AddHours(-8) - start).TotalSeconds * 1000);
        }

        /// <summary>
        /// unix时间戳转换成日期,13位
        /// </summary>
        /// <param name="unixTimeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(DateTime target, long timestamp)
        {
            timestamp = timestamp / 1000;
            var start = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            return start.AddSeconds(timestamp).AddHours(8);
        }
    }
}
