using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Config.DTO
{
    public class QueryConfigOutput: BaseOutput
    {
        public string AppName { get; set; }
        /// <summary>
        /// 当前最大版本
        /// </summary>
        public long Version { get; set; }
        /// <summary>
        /// Key/Value
        /// </summary>
        public Dictionary<string, string> KV { set; get; }
    }
}
