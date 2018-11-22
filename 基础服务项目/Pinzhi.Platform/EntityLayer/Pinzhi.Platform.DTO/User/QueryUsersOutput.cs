using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Dto
{
    public class QueryUsersOutput: BasePageOutput
    {
        public List<QueryUserDTO> Data { set; get; }
    }
    public class QueryUserDTO
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
        /// 账户状态
        /// </summary>
        public int State { set; get; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { set; get; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public dynamic RoleList { set; get; }
    }
}
