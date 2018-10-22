using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Model
{
    /// <summary>
    /// 用户信息实体类
    /// </summary>
    [SugarTable("tb_users")]
    public class UserInfo
    {
        /// <summary>
        /// udcid 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Id { set; get; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 账号密码
        /// </summary>
        public string Password { set; get; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 盐值
        /// </summary>
        public string Salt { set; get; }
        /// <summary>
        /// 账户状态
        /// </summary>
        public int State { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { set; get; }
    }
}
