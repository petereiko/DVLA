using System.Linq.Expressions;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        T Add(T entity);
        Task<T> AddAsync(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        bool Any(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        void BeginTransaction();
        Task BeginTransactionAsync();
        void CommitTransaction();
        Task CommitTransactionAsync();
        IEnumerable<T> ExecuteStoredProcedure(string storedProcedureName, params object[] parameters);
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedureName, params object[] parameters);
        int ExecuteStoredProcedureNonQuery(string storedProcedureName, params object[] parameters);
        Task<int> ExecuteStoredProcedureNonQueryAsync(string storedProcedureName, params object[] parameters);
        IEnumerable<T> Filter(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate, bool isTracking = true, int? take = null, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAll(bool isTracking = true, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(bool isTracking = true, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetLastRecords(int count, bool isTracking = true, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetLastRecordsAsync(int count, bool isTracking = true, params Expression<Func<T, object>>[] includes);
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        T GetSingle(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes);
        void Remove(T entity);
        Task RemoveAsync(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task RemoveRangeAsync(IEnumerable<T> entities);
        void RollbackTransaction();
        Task RollbackTransactionAsync();
        void Update(T entity);
        Task UpdateAsync(T entity);
    }
}
