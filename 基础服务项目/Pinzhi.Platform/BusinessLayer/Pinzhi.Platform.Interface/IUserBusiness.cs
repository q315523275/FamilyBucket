using Pinzhi.Platform.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pinzhi.Platform.Interface
{
    public interface IUserBusiness
    {
        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<QueryUsersOutput> QueryUsers(QueryUsersInput input);
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<SetUserOutput> SetUser(SetUserInput input);
    }
}
