using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Gprc.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGrpcConsulRegisterService(this IApplicationBuilder app, IConfiguration configuration)
        {
            return app;
        }
    }
}
