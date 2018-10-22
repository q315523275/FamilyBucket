using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Values
{
    public class HostAndPort
    {
        public HostAndPort(string downstreamHost, int downstreamPort)
        {
            Address = downstreamHost?.Trim('/');
            Port = downstreamPort;
        }

        public string Address { get; private set; }
        public int Port { get; private set; }
        public Uri ToUri(string scheme = "http", string path = "/")
        {
            var builder = new UriBuilder(scheme, this.Address, this.Port, path);
            return builder.Uri;
        }
        public override string ToString()
        {
            return $"{this.Address}:{this.Port}";
        }
    }
}
