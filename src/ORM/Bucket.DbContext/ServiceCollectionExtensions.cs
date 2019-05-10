using Bucket.DbContext.SqlSugar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.DbContext
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加多数据库 DbContext
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSqlSugarDbContext(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
        {
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            var connectOptions = configuration.GetSection("DbConfig").Get<List<DbConnectOption>>();
            if (connectOptions != null)
            {
                foreach (var option in connectOptions)
                {
                    if (contextLifetime == ServiceLifetime.Scoped)
                        services.AddScoped(s =>
                        {
                            return new BucketSqlSugarClient(new ConnectionConfig
                            {
                                ConnectionString = option.ConnectionString,
                                DbType = option.DbType,
                                IsAutoCloseConnection = option.IsAutoCloseConnection,
                                InitKeyType = InitKeyType.Attribute
                            }, option.Name, option.Default);
                        });
                    if (contextLifetime == ServiceLifetime.Singleton)
                        services.AddSingleton(s =>
                        {
                            return new BucketSqlSugarClient(new ConnectionConfig
                            {
                                ConnectionString = option.ConnectionString,
                                DbType = option.DbType,
                                IsAutoCloseConnection = option.IsAutoCloseConnection,
                                InitKeyType = InitKeyType.Attribute
                            }, option.Name, option.Default);
                        });
                    if (contextLifetime == ServiceLifetime.Transient)
                        services.AddTransient(s =>
                        {
                            return new BucketSqlSugarClient(new ConnectionConfig
                            {
                                ConnectionString = option.ConnectionString,
                                DbType = option.DbType,
                                IsAutoCloseConnection = option.IsAutoCloseConnection,
                                InitKeyType = InitKeyType.Attribute
                            }, option.Name, option.Default);
                        });
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
    }
}
