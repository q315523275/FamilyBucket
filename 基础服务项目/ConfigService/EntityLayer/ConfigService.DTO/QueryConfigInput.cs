using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigService.DTO
{
    public class QueryConfigInput
    {
        [NotEmpty("config_001","AppId不能为空")]
        public string AppId { set; get; }
        public long Version { set; get; }
        [NotEmpty("config_002", "签名不能为空")]
        public string Sign { set; get; }
        [NotEmpty("config_005", "NamespaceName不能为空")]
        public string NamespaceName { set; get; }
        public string Env { set; get; }
    }
}
