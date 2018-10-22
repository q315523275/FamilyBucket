using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class SetUserInput
    {
        /// <summary>
        /// udcid 
        /// </summary>
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
        [NotEmpty("GO_0005013","姓名不能为空")]
        public string RealName { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        [NotEmpty("GO_0005002","手机号不能为空")]
        [Mobile("GO_0005003","不是有效手机号")]
        public string Mobile { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 账户状态
        /// </summary>
        public int State { set; get; }
        /// <summary>
        /// 盐值
        /// </summary>
        public string Salt { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { set; get; }
        /// <summary>
        /// 角色数组
        /// </summary>
        public List<int> RoleIdList { set; get; }
    }
}
