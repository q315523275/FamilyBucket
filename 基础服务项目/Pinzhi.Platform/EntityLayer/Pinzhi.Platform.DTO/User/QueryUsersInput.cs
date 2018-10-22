using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class QueryUsersInput: BasePageInput
    {
        /// <summary>
        /// 用户状态
        /// </summary>
        public int State { set; get; }
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Mobile("104002","不是有效手机号")]
        public string Mobile { set; get; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { set; get; }
        /// <summary>
        /// 角色
        /// </summary>
        public int RoleId { set; get; }
    }
}
