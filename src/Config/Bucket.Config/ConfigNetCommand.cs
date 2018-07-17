using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Config
{
    /// <summary>
    /// 网络命令
    /// </summary>
    public class ConfigNetCommand
    {
        public EnumCommandType CommandType { get; set; }
        public string AppId { get; set; }
    }
    /// <summary>
    /// 网络命令类型
    /// </summary>
    public enum EnumCommandType
    {
        /// <summary>
        /// 配置更新
        /// </summary>
        ConfigUpdate,
        /// <summary>
        /// 配置重新加载
        /// </summary>
        ConfigReload
    }
}
