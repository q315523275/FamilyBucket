using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Bucket.Logging.Nlog
{
    public static class ServiceCollectionExtensions
    {
        public static ILoggerFactory AddNLog(this ILoggerFactory factory, string nlogConfigFile = "nlog.config")
        {
            NLog.LogManager.LoadConfiguration(nlogConfigFile);
            factory.AddProvider(new NLogProvider());
            return factory;
        }
        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, string nlogConfigFile = "nlog.config")
        {
            builder.Services.AddSingleton<ILoggerProvider>(sp => new NLogProvider());
            return builder;
        }
        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, Func<string, LogLevel, bool> filter, string nlogConfigFile = "nlog.config")
        {
            builder.AddFilter(filter);
            builder.Services.AddSingleton<ILoggerProvider>(sp => new NLogProvider());
            return builder;
        }
    }
}
