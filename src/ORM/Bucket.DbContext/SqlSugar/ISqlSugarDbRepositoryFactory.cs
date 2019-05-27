namespace Bucket.DbContext.SqlSugar
{
    /// <summary>
    /// SqlSugar数据库仓储获取工厂,方法暂未验证,尽量直接使用构造函数
    /// </summary>
    public interface ISqlSugarDbRepositoryFactory
    {
        ISqlSugarDbRepository<TEntity> Get<TEntity>() where TEntity : class, new();
    }
}
