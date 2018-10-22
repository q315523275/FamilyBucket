using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class SetAppConfigInfoInput
    {
        public int Id { set; get; }
        public string ConfigAppId { set; get; }
        public string ConfigNamespaceName { set; get; }
        public string ConfigKey { set; get; }
        public string ConfigValue { set; get; }
        public string Remark { set; get; }
        public bool IsDeleted { set; get; }
        /// <summary>
        /// 环境
        /// </summary>
        public string Environment { set; get; }
    }
}
