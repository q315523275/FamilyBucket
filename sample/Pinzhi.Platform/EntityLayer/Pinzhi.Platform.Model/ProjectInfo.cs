using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Model
{
    /// <summary>
    /// 项目实体类
    /// </summary>
    [SugarTable("tb_projects")]
    public class ProjectInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { set; get; }
        /// <summary>
        /// 平台Key
        /// </summary>
        public string Key { set; get; }
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
        /// <summary>
        /// 关联配置中心分类编号
        /// </summary>
        public string ConfigCateId { set; get; }
        /// <summary>
        /// 路由前缀
        /// </summary>
        public string RouteKey { set; get; }
    }
}
