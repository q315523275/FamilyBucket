using SqlSugar;
using System.Collections.Generic;

namespace Bucket.DbContext.SqlSugar
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class SqlSugarDbConnectOption : ConnectionConfig
    {
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 是否默认库
        /// </summary>
        public bool Default { set; get; } = true;
    }
}
