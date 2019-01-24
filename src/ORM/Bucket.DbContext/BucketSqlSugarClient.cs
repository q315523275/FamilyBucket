using SqlSugar;

namespace Bucket.DbContext
{
    public class BucketSqlSugarClient : SqlSugarClient
    {
        public BucketSqlSugarClient(ConnectionConfig config) : base(config) { DbName = string.Empty; Default = true; }
        public BucketSqlSugarClient(ConnectionConfig config, string dbName) : base(config) { DbName = dbName; Default = true; }
        public BucketSqlSugarClient(ConnectionConfig config, string dbName, bool isDefault) : base(config) { DbName = dbName; Default = isDefault; }
        public string DbName { set; get; }
        public bool Default { set; get; }
    }
}
