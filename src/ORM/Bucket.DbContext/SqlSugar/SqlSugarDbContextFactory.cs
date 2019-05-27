using System;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.DbContext.SqlSugar
{
    public class SqlSugarDbContextFactory : ISqlSugarDbContextFactory
    {
        private readonly IEnumerable<BucketSqlSugarClient> _clients;
        public SqlSugarDbContextFactory(IEnumerable<BucketSqlSugarClient> clients)
        {
            _clients = clients;
        }

        public BucketSqlSugarClient Get(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var dbContext = _clients.FirstOrDefault(it => it.DbName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (dbContext == null)
                throw new ArgumentException("can not find a match dbcontext!");

            return dbContext;
        }
    }
}
