using DVLA.VerificationPortal.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        T GetById(object id);

        Task<PaginatedResponse<T>> GetPagedAsync(Expression<Func<T, bool>>? filter, int pageIndex, int pageSize, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllAsync(bool isTracking = true, params Expression<Func<T, object>>[] includes);


        IEnumerable<T> GetAll(bool isTracking = true, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> FilterAsync(
    Expression<Func<T, bool>> predicate,
    bool isTracking = true,
    int? take = null,
    params Expression<Func<T, object>>[] includes);

        //Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> Filter(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes);


        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes);
        T GetSingle(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes);


        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);


        Task<T> AddAsync(T entity);
        T Add(T entity);


        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        IEnumerable<T> AddRange(IEnumerable<T> entities);

        Task UpdateAsync(T entity);
        void Update(T entity);


        Task RemoveAsync(T entity);
        void Remove(T entity);


        Task RemoveRangeAsync(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);

        Task BeginTransactionAsync();
        void BeginTransaction();

        Task CommitTransactionAsync();
        void CommitTransaction();

        Task RollbackTransactionAsync();
        void RollbackTransaction();

        #region Stored Procedures

        Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedureName, params object[] parameters);
        IEnumerable<T> ExecuteStoredProcedure(string storedProcedureName, params object[] parameters);


        Task<int> ExecuteStoredProcedureNonQueryAsync(string storedProcedureName, params object[] parameters);
        int ExecuteStoredProcedureNonQuery(string storedProcedureName, params object[] parameters);

        #endregion
    }
}
