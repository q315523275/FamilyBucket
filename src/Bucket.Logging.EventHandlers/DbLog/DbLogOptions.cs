using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Logging.EventHandlers
{
    public class DbLogOptions
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 是否输出到控制台
        /// </summary>
        public bool IsWriteConsole { get; set; }
        /// <summary>
        /// 是否进行数据库分片
        /// </summary>
        public bool IsDbSharding { get; set; }
        /// <summary>
        /// 数据库分片方式(1按天，2按月)
        /// </summary>
        public int DbShardingRule { get; set; }
    }
}
