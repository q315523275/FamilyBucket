namespace Bucket.DbContext.SqlSugar
{
    public interface ISqlSugarDbContextFactory
    {
        BucketSqlSugarClient Get(string name);
    }
}
