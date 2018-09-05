using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bucket.DbContext
{
    public class RepositoryBase<T> : SimpleClient<T>, IRepositoryBase<T> where T : class, new()
    {
        public RepositoryBase(SqlSugarClient context) : base(context) { }


        public SqlSugarClient DbContext { get { return FullClient; } }

        public async Task<List<T>> GetListAsync()
        {
            return await Context.Queryable<T>().ToListAsync();
        }
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await Context.Queryable<T>().Where(whereExpression).ToListAsync();
        }
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await Context.Queryable<T>().SingleAsync(whereExpression);
        }
        public async Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            int count = 0;
            var result = await Context.Queryable<T>().Where(whereExpression).ToPageListAsync(page.PageIndex, page.PageSize, count);
            page.PageCount = result.Value;
            return result.Key;
        }
        public async Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = await Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageListAsync(page.PageIndex, page.PageSize, count);
            page.PageCount = result.Value;
            return result.Key;
        }
        public async Task<List<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page)
        {
            int count = 0;
            var result = await Context.Queryable<T>().Where(conditionalList).ToPageListAsync(page.PageIndex, page.PageSize, count);
            page.PageCount = result.Value;
            return result.Key;
        }
        public async Task<List<T>> GetPageListAsync(List<IConditionalModel> conditionalList, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = await Context.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(conditionalList).ToPageListAsync(page.PageIndex, page.PageSize, count);
            page.PageCount = result.Value;
            return result.Key;
        }
        public async Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await Context.Queryable<T>().Where(whereExpression).AnyAsync();
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await Context.Queryable<T>().Where(whereExpression).CountAsync();
        }

        public async Task<bool> InsertAsync(T insertObj)
        {
            return await Context.Insertable(insertObj).ExecuteCommandAsync() > 0;
        }
        public async Task<long> InsertReturnIdentityAsync(T insertObj)
        {
            return await Context.Insertable(insertObj).ExecuteReturnBigIdentityAsync();
        }
        public async Task<bool> InsertRangeAsync(T[] insertObjs)
        {
            return await Context.Insertable(insertObjs).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> InsertRangeAsync(List<T>[] insertObjs)
        {
            return await Context.Insertable(insertObjs).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateAsync(T updateObj)
        {
            return await Context.Updateable(updateObj).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateRangeAsync(T[] updateObjs)
        {
            return await Context.Updateable(updateObjs).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return await Context.Updateable<T>().UpdateColumns(columns).Where(whereExpression).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> DeleteAsync(T deleteObj)
        {
            return await Context.Deleteable<T>().Where(deleteObj).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await Context.Deleteable<T>().Where(whereExpression).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> DeleteByIdAsync(dynamic id)
        {
            return await Context.Deleteable<T>().In(id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> DeleteByIdsAsync(dynamic[] ids)
        {
            return await Context.Deleteable<T>().In(ids).ExecuteCommandAsync() > 0;
        }
    }
}
