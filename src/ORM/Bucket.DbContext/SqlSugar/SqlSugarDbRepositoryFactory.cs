using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
namespace Bucket.DbContext.SqlSugar
{
    public class SqlSugarDbRepositoryFactory : ISqlSugarDbRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public SqlSugarDbRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISqlSugarDbRepository<TEntity> Get<TEntity>() where TEntity : class, new()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetRequiredService<ISqlSugarDbRepository<TEntity>>();
            }
        }
    }
}
