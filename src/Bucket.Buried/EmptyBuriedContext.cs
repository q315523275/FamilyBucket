using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Bucket.Buried
{
    public class EmptyBuriedContext : IBuriedContext
    {
        public EmptyBuriedContext()
        {
        }

        public Dictionary<string, string> DownStreamHeaders()
        {
            return new Dictionary<string, string>();
        }

        public Task PublishAsync<T>(T buriedInformation)
        {
            return Task.FromResult(0);
        }

        public Task PublishAsync<T>(T buriedInformation, HttpContext httpContext)
        {
            return Task.FromResult(0);
        }
    }
}
