using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigService.Model
{
    /// <summary>
    /// 项目实体类
    /// </summary>
    [SugarTable("tb_app")]
    public class AppInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { set; get; }
        /// <summary>
        /// 平台AppId
        /// </summary>
        public string AppId { set; get; }
        /// <summary>
        /// 应用平台
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { set; get; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { set; get; }
    }
}
