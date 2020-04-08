using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Bucket.EventBus.Util
{
    public static class DnsHelper
    {
        /// <summary>
        /// 获取本地的IP地址
        /// </summary>
        public static string GetLanIp()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                            .Select(p => p.GetIPProperties())
                            .SelectMany(p => p.UnicastAddresses)
                            .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))
                            .FirstOrDefault()?.Address.ToString();
        }
    }
}