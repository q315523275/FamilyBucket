using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SqlSugar;
using System;

namespace Bucket.DbContext
{
    /// <summary>
    /// SqlSugar 注入Service的扩展方法 <see cref="IServiceCollection" />.
    /// SqlSugar 和 Dbcontext 不同，没有DbContext池 所有的 AddSQLSugarClientPool 已全部删除
    /// SqlSugarClient 没有无参的构造函数 configAction 可为空的方法已注释
    /// </summary>
    public static class SqlSugarServiceCollectionExtensions
    {
        /// <summary>
        ///     将给定的上下文作为服务注册在<see cref ="IServiceCollection"/>中
        ///     在应用程序中使用依赖注入时，如ASP.NET使用此方法。
        ///     有关设置依赖注入的更多信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=526890。
        /// </summary>
        /// <example>
        ///     <code>
        ///          services.AddSQLSugarClient&lt;SqlSugarClient&gt;(config =>
        ///          {
        ///              config.ConnectionString = connectionString;
        ///              config.DbType = DbType.MySql;
        ///              config.IsAutoCloseConnection = true;
        ///              config.InitKeyType = InitKeyType.Attribute;
        ///          });
        ///      </code>
        /// </example>
        /// <typeparam name="TSugarClient"> 要注册的上下文的类型。 </typeparam>
        /// <param name="serviceCollection"> <see cref="IServiceCollection" /> 添加服务。 </param>
        /// <param name="configAction">
        ///     <para>
        ///         为上下文配置<see cref ="ConnectionConfig"/> 的操作。
        ///     </para>
        ///     <para>
        ///         为了将选项传递到上下文中，需要在您的上下文中公开构造函数
        ///         <see cref="ConnectionConfig" /> and passes it to the base constructor of <see cref="TSugarClient" />.
        ///     </para>
        /// </param>
        /// <param name="contextLifetime"> 用于在容器中注册TSugarClient服务的生命周期。 </param>
        /// <param name="configLifetime"> 用于在容器中注册ConnectionConfig服务的生命周期。 </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddSQLSugarClient<TSugarClient>(
            this IServiceCollection serviceCollection,
            Action<ConnectionConfig> configAction,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime configLifetime = ServiceLifetime.Scoped)
            where TSugarClient : SqlSugarClient
            => AddSQLSugarClient<TSugarClient>(
                serviceCollection,
                (p, b) => configAction.Invoke(b), contextLifetime, configLifetime);

        /// <summary>
        ///     <para>
        ///         将给定的上下文作为服务注册在<see cref ="IServiceCollection"/>中。
        ///         在应用程序中使用依赖注入时，如ASP.NET使用此方法。
        ///         有关设置依赖注入的更多信息，请参阅http://go.microsoft.com/fwlink/?LinkId=526890。
        ///     </para>
        /// </summary>
        /// <typeparam name="TSugarClient"> 要注册的上下文的类型。 </typeparam>
        /// <param name="serviceCollection"> <see cref="IServiceCollection" /> 添加服务。 </param>
        /// <param name="configAction">
        ///     <para>
        ///         为上下文配置<see cref ="ConnectionConfig"/> 的操作。
        ///     </para>
        ///     <para>
        ///         为了将选项传递到上下文中，需要在您的上下文中公开构造函数
        ///         <see cref="ConnectionConfig" /> and passes it to the base constructor of <see cref="TSugarClient" />.
        ///     </para>
        /// </param>
        /// <param name="contextLifetime"> 用于在容器中注册TSugarClient服务的生命周期。 </param>
        /// <param name="configLifetime"> 用于在容器中注册ConnectionConfig服务的生命周期。 </param>
        /// <returns>
        ///     The same service collection so that multiple calls can be chained.
        /// </returns>
        public static IServiceCollection AddSQLSugarClient<TSugarClient>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, ConnectionConfig> configAction,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime configLifetime = ServiceLifetime.Scoped)
            where TSugarClient : SqlSugarClient
        {
            if (contextLifetime == ServiceLifetime.Singleton)
            {
                configLifetime = ServiceLifetime.Singleton;
            }

            AddCoreServices<TSugarClient>(serviceCollection, configAction, configLifetime);

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(TSugarClient), typeof(TSugarClient), contextLifetime));

            return serviceCollection;
        }
        private static void AddCoreServices<TSugarClient>(
            IServiceCollection serviceCollection,
            Action<IServiceProvider, ConnectionConfig> configAction,
            ServiceLifetime configLifetime)
            where TSugarClient : SqlSugarClient
        {

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(ConnectionConfig),
                    p => ConnectionConfigFactory(p, configAction),
                    configLifetime));
        }

        private static ConnectionConfig ConnectionConfigFactory(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, ConnectionConfig> configAction)
        {
            var config = new ConnectionConfig();

            configAction.Invoke(applicationServiceProvider, config);

            return config;
        }
    }
}
