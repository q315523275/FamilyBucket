using SqlSugar;

namespace Bucket.DbContext.SqlSugar
{
    public class BucketSqlSugarClient : SqlSugarClient
    {
        public BucketSqlSugarClient(SqlSugarDbConnectOption config) : base(config) { DbName = config.Name; Default = config.Default; }
        public string DbName { set; get; }
        public bool Default { set; get; }
    }
}
