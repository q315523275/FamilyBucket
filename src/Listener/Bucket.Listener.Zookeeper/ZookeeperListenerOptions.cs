using Rabbit.Zookeeper;
namespace Bucket.Listener.Zookeeper
{
    public class ZookeeperListenerOptions : ZookeeperClientOptions
    {
        public ZookeeperListenerOptions()
        {
            BasePath = "/";
        }

        public string ListenerKey { set; get; }
    }
}
