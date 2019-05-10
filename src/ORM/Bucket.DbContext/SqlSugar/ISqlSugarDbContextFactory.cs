namespace Bucket.DbContext
{
    public interface ISqlSugarDbContextFactory
    {
        BucketSqlSugarClient Get(string name);
    }
}
