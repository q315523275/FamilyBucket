using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class SetServiceInfoInput
    {
        public string Name { set; get; }
        public string Version { set; get; }
        public string[] Tags { set; get; }
        public HostAndPortDto HostAndPort { set; get; }
    }
    public class HostAndPortDto
    {
        public string Host { set; get; }
        public string Port { set; get; }
    }
}
