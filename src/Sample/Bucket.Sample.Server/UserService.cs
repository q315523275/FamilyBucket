using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Sample.Server
{
    public class UserService : IUserService
    {
        #region Implementation of IUserService

        public Task<string> GetUserName(int id)
        {
            return Task.FromResult($"id:{id} is name rabbit.");
        }

        public Task<bool> Exists(int id)
        {
            return Task.FromResult(true);
        }

        public Task<int> GetUserId(string userName)
        {
            return Task.FromResult(1);
        }

        public Task<DateTime> GetUserLastSignInTime(int id)
        {
            return Task.FromResult(DateTime.Now);
        }

        public Task<UserModel> GetUser(int id)
        {
            return Task.FromResult(new UserModel
            {
                Name = "rabbit",
                Age = 18
            });
        }

        public Task<bool> Update(int id, UserModel model)
        {
            return Task.FromResult(true);
        }

        public Task<IDictionary<string, string>> GetDictionary()
        {
            return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string> { { "key", "value" } });
        }

        public async Task Try()
        {
            Console.WriteLine("start");
            await Task.Delay(5000);
            Console.WriteLine("end");
        }

        public Task TryThrowException()
        {
            throw new Exception("我是服务端的异常");
        }

        #endregion Implementation of IUserService
    }
}
