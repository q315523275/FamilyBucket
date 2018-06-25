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
        public DateTime LastTime { set; get; }
        public DateTime CreateTime { set; get; }
        public long Version { set; get; }
        public bool IsDeleted { set; get; }
    }
}
