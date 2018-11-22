using SqlSugar;
using System;

namespace Pinzhi.Platform.Model
{
    /// <summary>
    /// 角色实体类
    /// </summary>
    [SugarTable("tb_roles")]
    public class RoleInfo
    {
        /// <summary>
        /// 角色唯一标示符
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatformKey { get; set; }
        /// <summary>
        /// 角色唯一标示符
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        ///角色名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否系统角色
        /// </summary>
        public bool IsSys { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
    }
}
