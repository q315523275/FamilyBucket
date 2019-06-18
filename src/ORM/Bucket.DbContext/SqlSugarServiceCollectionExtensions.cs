using Bucket.DbContext.SqlSugar;
using Bucket.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.DbContext
{
    public static class SqlSugarServiceCollectionExtensions
    {
        /// <summary>
        /// 添加sqlSugar多数据库支持
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugarDbContext(this IServiceCollection services, IConfiguration configuration, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, string section = "DbConfig")
        {
            var connectOptions = configuration.GetSection(section).Get<List<SqlSugarDbConnectOption>>();
            if (connectOptions != null)
            {
                foreach (var option in connectOptions)
                {
                    if (contextLifetime == ServiceLifetime.Scoped)
                        services.AddScoped(s => new BucketSqlSugarClient(option));
                    if (contextLifetime == ServiceLifetime.Singleton)
                        services.AddSingleton(s => new BucketSqlSugarClient(option));
                    if (contextLifetime == ServiceLifetime.Transient)
                        services.AddTransient(s => new BucketSqlSugarClient(option));
                }
                if (contextLifetime == ServiceLifetime.Singleton)
                    services.AddSingleton<ISqlSugarDbContextFactory, SqlSugarDbContextFactory>();
                if (contextLifetime == ServiceLifetime.Scoped)
                    services.AddScoped<ISqlSugarDbContextFactory, SqlSugarDbContextFactory>();
                if (contextLifetime == ServiceLifetime.Transient)
                    services.AddTransient<ISqlSugarDbContextFactory, SqlSugarDbContextFactory>();
            }
            return services;
        }

        /// <summary>
        /// 添加sqlSugar多数据库支持
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugarDbContext(this IBucketBuilder builder, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, string section = "DbConfig")
        {
            return AddSqlSugarDbContext(builder.Services, builder.Configuration, contextLifetime, section);
        }

        /// <summary>
        /// 添加sqlSugar多数据库支持
        /// </summary>
        /// <param name="services"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugarDbContext(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, string section = "DbConfig")
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            return AddSqlSugarDbContext(services, configuration, contextLifetime, section);
        }

        /// <summary>
        /// 添加sqlSugar数据仓储,依赖AddSqlSugarDbContext
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugarDbRepository(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            if (contextLifetime == ServiceLifetime.Singleton)
                services.AddSingleton(typeof(ISqlSugarDbRepository<>), typeof(SqlSugarDbRepository<>));
            if (contextLifetime == ServiceLifetime.Scoped)
                services.AddScoped(typeof(ISqlSugarDbRepository<>), typeof(SqlSugarDbRepository<>));
            if (contextLifetime == ServiceLifetime.Transient)
                services.AddTransient(typeof(ISqlSugarDbRepository<>), typeof(SqlSugarDbRepository<>));
            services.AddSingleton<ISqlSugarDbRepositoryFactory, SqlSugarDbRepositoryFactory>();
            return services;
        }
    }
}
