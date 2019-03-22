using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Bucket.Logging.Log4Net
{
    public static class ServiceCollectionExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile = "log4net.config")
        {
            factory.AddProvider(new Log4NetProvider(log4NetConfigFile));
            return factory;
        }
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder builder, string log4NetConfigFile = "log4net.config")
        {
            builder.Services.AddSingleton<ILoggerProvider>(sp => new Log4NetProvider(log4NetConfigFile));
            return builder;
        }
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder builder, Func<string, LogLevel, bool> filter, string log4NetConfigFile = "log4net.config")
        {
            builder.AddFilter(filter);
            builder.Services.AddSingleton<ILoggerProvider>(sp => new Log4NetProvider(log4NetConfigFile));
            return builder;
        }
    }
}
