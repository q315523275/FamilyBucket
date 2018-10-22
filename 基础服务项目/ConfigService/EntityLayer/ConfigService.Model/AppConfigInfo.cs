using SqlSugar;
using System;

namespace ConfigService.Model
{
    /// <summary>
    /// 配置信息
    /// </summary>
    [SugarTable("tb_appconfig")]
    public class AppConfigInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { set; get; }
        public string ConfigAppId { set; get; }
        public string ConfigNamespaceName { set; get; }
        public string ConfigKey { set; get; }
        public string ConfigValue { set; get; }
        public string Remark { set; get; }
        public bool IsDeleted { set; get; }
        public DateTime LastTime { set; get; }
        public DateTime CreateTime { set; get; }
        public long Version { get; set; }
    }
}
