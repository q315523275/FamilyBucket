using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Model
{
    /// <summary>
    /// 角色接口实体类
    /// </summary>
    [SugarTable("tb_role_apis")]
    public class RoleApiInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// Api资源唯一ID
        /// </summary>
        public int ApiId { get; set; }
        /// <summary>
        /// 角色唯一ID
        /// </summary>
        public int RoleId { get; set; }
    }
}
