using Bucket.Rpc.Server.ServiceDiscovery.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Sample
{
    [ProtoContract]
    public class UserModel
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int Age { get; set; }
    }

    [RpcServiceBundle]
    public interface IUserService
    {
        [RpcService(Route = "/User/GetUserName")]
        Task<string> GetUserName(int id);

        Task<bool> Exists(int id);

        Task<int> GetUserId(string userName);

        Task<DateTime> GetUserLastSignInTime(int id);

        Task<UserModel> GetUser(int id);

        Task<bool> Update(int id, UserModel model);

        Task<IDictionary<string, string>> GetDictionary();

        [RpcService(IsWaitExecution = false, Route = "/User/Try")]
        Task Try();

        [RpcService(Route = "/User/TryThrowException")]
        Task TryThrowException();
    }
}
