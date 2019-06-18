using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bucket.DbContext.SqlSugar
{
    public class SqlSugarDbRepository<T> : ISqlSugarDbRepository<T> where T : class, new()
    {
        private readonly IEnumerable<BucketSqlSugarClient> _clients;

        public SqlSugarDbRepository(IEnumerable<BucketSqlSugarClient> clients)
        {
            _clients = clients;
            DbContext = _clients.FirstOrDefault(it => it.Default);
        }

        private BucketSqlSugarClient DbContext { get; set; }

        public ISugarQueryable<T> AsQueryable()
        {
            return DbContext.Queryable<T>();
        }
        public IInsertable<T> AsInsertable(T insertObj)
        {
            return DbContext.Insertable<T>(insertObj);
        }
        public IInsertable<T> AsInsertable(T[] insertObjs)
        {
            return DbContext.Insertable<T>(insertObjs);
        }
        public IInsertable<T> AsInsertable(List<T> insertObjs)
        {
            return DbContext.Insertable<T>(insertObjs);
        }
        public IUpdateable<T> AsUpdateable(T updateObj)
        {
            return DbContext.Updateable<T>(updateObj);
        }
        public IUpdateable<T> AsUpdateable(T[] updateObjs)
        {
            return DbContext.Updateable<T>(updateObjs);
        }
        public IUpdateable<T> AsUpdateable(List<T> updateObjs)
        {
            return DbContext.Updateable<T>(updateObjs);
        }
        public IDeleteable<T> AsDeleteable()
        {
            return DbContext.Deleteable<T>();
        }

        public void BeginTran()
        {
            DbContext.Ado.BeginTran();
        }

        public void CommitTran()
        {
            DbContext.Ado.CommitTran();
        }

        public int Count(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().Count(whereExpression);
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().CountAsync(whereExpression);
        }

        public bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Deleteable<T>().Where(whereExpression).ExecuteCommand() > 0;
        }

        public bool Delete(T deleteObj)
        {
            return DbContext.Deleteable(deleteObj).ExecuteCommand() > 0;
        }

        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await DbContext.Deleteable<T>().Where(whereExpression).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> DeleteAsync(T deleteObj)
        {
            return await DbContext.Deleteable(deleteObj).ExecuteCommandAsync() > 0;
        }

        public bool DeleteById(dynamic id)
        {
            return DbContext.Deleteable<T>().In(id).ExecuteCommand() > 0;
        }

        public async Task<bool> DeleteByIdAsync(dynamic id)
        {
            return await DbContext.Deleteable<T>().In(id).ExecuteCommandAsync() > 0;
        }

        public bool DeleteByIds(dynamic[] ids)
        {
            return DbContext.Deleteable<T>().In(ids).ExecuteCommand() > 0;
        }

        public async Task<bool> DeleteByIdsAsync(dynamic[] ids)
        {
            return await DbContext.Deleteable<T>().In(ids).ExecuteCommandAsync() > 0;
        }

        public T GetById(dynamic id)
        {
            return DbContext.Queryable<T>().InSingle(id);
        }

        public T GetFirst(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().First(whereExpression);
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await DbContext.Queryable<T>().FirstAsync(whereExpression);
        }

        public List<T> GetList()
        {
            return DbContext.Queryable<T>().ToList();
        }

        public List<T> GetList(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().Where(whereExpression).ToList();
        }

        public List<T> GetList(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return DbContext.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToList();
        }

        public Task<List<T>> GetListAsync()
        {
            return DbContext.Queryable<T>().ToListAsync();
        }

        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().Where(whereExpression).ToListAsync();
        }

        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return DbContext.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToListAsync();
        }

        public List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            int count = 0;
            var result = DbContext.Queryable<T>().Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }

        public List<T> GetPageList(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = DbContext.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageList(page.PageIndex, page.PageSize, ref count);
            page.PageCount = count;
            return result;
        }

        public async Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page)
        {
            int count = 0;
            var result = await DbContext.Queryable<T>().Where(whereExpression).ToPageListAsync(page.PageIndex, page.PageSize, count);
            page.PageCount = result.Value;
            return result.Key;
        }

        public async Task<List<T>> GetPageListAsync(Expression<Func<T, bool>> whereExpression, PageModel page, Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            int count = 0;
            var result = await DbContext.Queryable<T>().OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToPageListAsync(page.PageIndex, page.PageSize, count);
            page.PageCount = result.Value;
            return result.Key;
        }

        public T GetSingle(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().Single(whereExpression);
        }

        public Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().SingleAsync(whereExpression);
        }

        public bool Insert(T insertObj)
        {
            return DbContext.Insertable(insertObj).ExecuteCommand() > 0;
        }

        public async Task<bool> InsertAsync(T insertObj)
        {
            return await DbContext.Insertable(insertObj).ExecuteCommandAsync() > 0;
        }

        public bool InsertRange(List<T> insertObjs)
        {
            return DbContext.Insertable(insertObjs).ExecuteCommand() > 0;
        }

        public bool InsertRange(T[] insertObjs)
        {
            return DbContext.Insertable(insertObjs).ExecuteCommand() > 0;
        }

        public async Task<bool> InsertRangeAsync(List<T> insertObjs)
        {
            return await DbContext.Insertable(insertObjs).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> InsertRangeAsync(T[] insertObjs)
        {
            return await DbContext.Insertable(insertObjs).ExecuteCommandAsync() > 0;
        }

        public int InsertReturnIdentity(T insertObj)
        {
            return DbContext.Insertable(insertObj).ExecuteReturnIdentity();
        }

        public async Task<long> InsertReturnIdentityAsync(T insertObj)
        {
            return await DbContext.Insertable(insertObj).ExecuteReturnBigIdentityAsync();
        }

        public bool IsAny(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Queryable<T>().Any(whereExpression);
        }

        public async Task<bool> IsAnyAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await DbContext.Queryable<T>().AnyAsync(whereExpression);
        }

        public void RollbackTran()
        {
            DbContext.Ado.RollbackTran();
        }

        public bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Updateable<T>().UpdateColumns(columns).Where(whereExpression).ExecuteCommand() > 0;
        }

        public bool Update(T updateObj)
        {
            return DbContext.Updateable(updateObj).ExecuteCommand() > 0;
        }

        public async Task<bool> UpdateAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return await DbContext.Updateable<T>().UpdateColumns(columns).Where(whereExpression).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateAsync(T updateObj)
        {
            return await DbContext.Updateable(updateObj).ExecuteCommandAsync() > 0;
        }

        public bool UpdateRange(T[] updateObjs)
        {
            return DbContext.Updateable(updateObjs).ExecuteCommand() > 0;
        }

        public async Task<bool> UpdateRangeAsync(T[] updateObjs)
        {
            return await DbContext.Updateable(updateObjs).ExecuteCommandAsync() > 0;
        }

        public ISqlSugarDbRepository<T> UseDb(string dbName)
        {
            DbContext = _clients.FirstOrDefault(it => it.DbName == dbName);
            return this;
        }

        public DbResult<T2> UseTran<T2>(Func<T2> action)
        {
            return DbContext.Ado.UseTran(action);
        }

        public DbResult<bool> UseTran(Action action)
        {
            return DbContext.Ado.UseTran(action);
        }

        public async Task<DbResult<T2>> UseTranAsync<T2>(Func<T2> action)
        {
            return await DbContext.Ado.UseTranAsync(action);
        }

        public async Task<DbResult<bool>> UseTranAsync(Action action)
        {
            return await DbContext.Ado.UseTranAsync(action);
        }
    }
}
