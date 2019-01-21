using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bucket.DbContext
{
    [Obsolete("单库仓储操作,请使用最新IDbRepository支持多库")]
    public interface IRepositoryBase<T> where T : class, new()
    {

        SqlSugarClient DbContext { get; }

        int Count(Expression<Func<T, bool>> whereExpression);
        bool Delete(Expression<Func<T, bool>> whereExpression);
        bool Delete(T deleteObj);
        bool DeleteById(dynamic id);
        bool DeleteByIds(dynamic[] ids);
        T GetById(dynamic id);
        List<T> GetList();
        List<T> GetList(Expression<Func<T, bool>> whereExpression);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page);
        List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page);
        List<T> GetPageList(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        T GetSingle(Expression<Func<T, bool>> whereExpression);
        T GetFirst(Expression<Func<T, bool>> whereExpression);
        bool Insert(T insertObj);
        bool InsertRange(List<T> insertObjs);
        bool InsertRange(T[] insertObjs);
        int InsertReturnIdentity(T insertObj);
        bool IsAny(Expression<Func<T, bool>> whereExpression);
        bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression);
        bool Update(T updateObj);
        bool UpdateRange(T[] updateObjs);

        Task<int> CountAsync(Expression<Func<T, bool>> whereExpression);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression);
        Task<bool> DeleteAsync(T deleteObj);
        Task<bool> DeleteByIdAsync(dynamic id);
        Task<bool> DeleteByIdsAsync(dynamic[] ids);
        Task<List<T>> GetListAsync();
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression);
        Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page);
        Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        Task<List<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page);
        Task<List<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression);
        Task<bool> InsertAsync(T insertObj);
        Task<bool> InsertRangeAsync(List<T> insertObjs);
        Task<bool> InsertRangeAsync(T[] insertObjs);
        Task<long> InsertReturnIdentityAsync(T insertObj);
        Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression);
        Task<bool> UpdateAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression);
        Task<bool> UpdateAsync(T updateObj);
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
