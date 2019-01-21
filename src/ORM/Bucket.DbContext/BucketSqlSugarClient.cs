using SqlSugar;

namespace Bucket.DbContext
{
    public class BucketSqlSugarClient : SqlSugarClient
    {
        public BucketSqlSugarClient(ConnectionConfig config) : base(config) { }
        public BucketSqlSugarClient(ConnectionConfig config, string dbName) : base(config) { DbName = dbName; }
        public string DbName { set; get; }
    }
}
