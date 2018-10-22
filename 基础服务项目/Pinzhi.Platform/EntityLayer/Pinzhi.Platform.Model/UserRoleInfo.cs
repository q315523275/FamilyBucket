using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Model
{
    /// <summary>
    /// 用户角色实体类
    /// </summary>
    [SugarTable("tb_user_roles")]
    public class UserRoleInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 用户唯一UdcId
        /// </summary>
        public long Uid { get; set; }
        /// <summary>
        /// 角色唯一ID
        /// </summary>
        public int RoleId { get; set; }
    }
}
