using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.MVC.Models
{
    /// <summary>
    /// 用户分部信息表
    /// </summary>
    [SugarTable("tb_users")]
    public class UserInfo
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
        public string RealName { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 账户状态
        /// </summary>
        public int State { set; get; }
        /// <summary>
        /// 盐值
        /// </summary>
        public string Salt { set; get; }
        /// <summary>
        ///  省ID
        /// </summary>
        public string ProvinceId { set; get; }
        /// <summary>
        ///  市ID
        /// </summary>
        public string CityId { set; get; }
        /// <summary>
        ///  区ID
        /// </summary>
        public string DistrictId { set; get; }
        /// <summary>
        ///  省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        ///  市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        ///  区
        /// </summary>
        public string District { set; get; }
        /// <summary>
        /// 营业厅名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { set; get; }
    }
}
