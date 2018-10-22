using System;
using System.Collections.Generic;

namespace Bucket.Values
{
    public class ServiceInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public HostAndPort HostAndPort { get; set; }
        public Uri ToUri(string scheme = "http", string path = "/")
        {
            var builder = new UriBuilder(scheme, HostAndPort.Address, HostAndPort.Port, path);
            return builder.Uri;
        }

        public override string ToString()
        {
            return $"{HostAndPort.Address}:{HostAndPort.Port}";
        }
    }
}
