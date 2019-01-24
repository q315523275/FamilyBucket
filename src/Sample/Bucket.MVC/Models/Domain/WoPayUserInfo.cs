using SqlSugar;
using System;

namespace Pinzhi.Credit.Model
{
    [SugarTable("wopay_users")]
    public class WoPayUserInfo
    {
        /// <summary>
        /// UDC编号
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Uid { set; get; }
        public string Email { set; get; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 沃支付用户号
        /// </summary>
    }
}
