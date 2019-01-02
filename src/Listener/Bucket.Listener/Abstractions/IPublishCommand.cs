using Bucket.Values;
using System.Threading.Tasks;

namespace Bucket.Listener.Abstractions
{
    public interface IPublishCommand
    {
        /// <summary>
        /// 推送命令
        /// </summary>
        /// <param name="applicationCode"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        Task PublishCommandMessage(string applicationCode, NetworkCommand command);
    }
}
