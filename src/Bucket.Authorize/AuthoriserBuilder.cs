using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Authorize
{
    public class AuthoriserBuilder: IAuthoriserBuilder
    {
        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public AuthoriserBuilder(IServiceCollection services, IConfiguration configurationRoot)
        {
            Configuration = configurationRoot;
            Services = services;
        }
    }
}
