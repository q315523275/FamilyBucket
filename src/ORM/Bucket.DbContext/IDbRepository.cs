using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bucket.DbContext
{
    public interface IDbRepository<T> where T : class, new()
    {
        BucketSqlSugarClient DbContext { get; }
        IDbRepository<T> UseDb(string dbName);

        ISugarQueryable<T> AsQueryable();
        IInsertable<T> AsInsertable(T insertObj);
        IInsertable<T> AsInsertable(T[] insertObjs);
        IInsertable<T> AsInsertable(List<T> insertObjs);
        IUpdateable<T> AsUpdateable(T updateObj);
        IUpdateable<T> AsUpdateable(T[] updateObjs);
        IUpdateable<T> AsUpdateable(List<T> updateObjs);
        IDeleteable<T> AsDeleteable();


        List<T> GetList();
        Task<List<T>> GetListAsync();
        List<T> GetList(Expression<Func<T, bool>> whereExpression);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression);
        List<T> GetList(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page);
        Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);


        int Count(Expression<Func<T, bool>> whereExpression);
        Task<int> CountAsync(Expression<Func<T, bool>> whereExpression);
        T GetById(dynamic id);
        T GetSingle(Expression<Func<T, bool>> whereExpression);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression);
        T GetFirst(Expression<Func<T, bool>> whereExpression);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression);


        bool IsAny(Expression<Func<T, bool>> whereExpression);
        Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression);

        bool Insert(T insertObj);
        Task<bool> InsertAsync(T insertObj);
        bool InsertRange(List<T> insertObjs);
        Task<bool> InsertRangeAsync(List<T> insertObjs);
        bool InsertRange(T[] insertObjs);
        Task<bool> InsertRangeAsync(T[] insertObjs);
        int InsertReturnIdentity(T insertObj);
        Task<long> InsertReturnIdentityAsync(T insertObj);


        bool Delete(Expression<Func<T, bool>> whereExpression);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression);
        bool Delete(T deleteObj);
        Task<bool> DeleteAsync(T deleteObj);
        bool DeleteById(dynamic id);
        Task<bool> DeleteByIdAsync(dynamic id);
        bool DeleteByIds(dynamic[] ids);
        Task<bool> DeleteByIdsAsync(dynamic[] ids);


        bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression);
        Task<bool> UpdateAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression);
        bool Update(T updateObj);
        Task<bool> UpdateAsync(T updateObj);
        bool UpdateRange(T[] updateObjs);
        Task<bool> UpdateRangeAsync(T[] updateObjs);


        DbResult<T2> UseTran<T2>(Func<T2> action);
        DbResult<bool> UseTran(Action action);
        Task<DbResult<T2>> UseTranAsync<T2>(Func<T2> action);
        Task<DbResult<bool>> UseTranAsync(Action action);

        void BeginTran();
        void CommitTran();
        void RollbackTran();
    }
}
