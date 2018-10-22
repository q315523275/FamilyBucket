using System.IO;
using Microsoft.Extensions.Configuration;

namespace Tracing.Common
{
    public static class AddressHelpers
    {
        private const string defaultAddress = "http://localhost:9618";
        private const string addressKey = "serveraddress";

        public static string GetApplicationUrl(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())   
                .AddJsonFile("appsettings.json", true, false)
                .AddJsonFile("appsettings.Development.json", true, false)
                .AddJsonFile("appsettings.Production.json", true, false)
                .AddEnvironmentVariables();

            if (args != null)
            {
                configurationBuilder.AddCommandLine(args);
            }
            
            var configuration = configurationBuilder.Build();           
            return configuration[addressKey] ?? defaultAddress;
        }
    }
}