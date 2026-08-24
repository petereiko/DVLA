using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private IDbContextTransaction _transaction;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        //public async Task<PaginatedResponse<T>> GetPagedAsync(Expression<Func<T, bool>>? filter, int pageIndex, int pageSize, params Expression<Func<T, object>>[] includes)
        //{
        //    IQueryable<T> query = _dbSet.AsNoTracking();

        //    if (includes != null)
        //    {
        //        foreach (var include in includes)
        //        {
        //            query = query.Include(include);
        //        }
        //    }

        //    if (filter != null)
        //    {
        //        query = query.Where(filter);
        //    }

        //    int totalCount = await query.CountAsync();

        //    var items = await query
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return new PaginatedResponse<T>(items, totalCount, pageIndex, pageSize);
        //}

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool isTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = isTracking ? _dbSet : _dbSet.AsNoTracking();
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetLastRecordsAsync(int count, bool isTracking = true, params Expression<Func<T, object>>[] includes)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<T>();
            }

            IQueryable<T> query = isTracking ? _dbSet : _dbSet.AsNoTracking();
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return await OrderByIdDescending(query).Take(count).ToListAsync();
        }




        public async Task<IEnumerable<T>> FilterAsync(
    Expression<Func<T, bool>> predicate,
    bool isTracking = true,
    int? take = null,
    params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = isTracking ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }


        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = isTracking ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);
            if (includes != null)
            {

                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().AnyAsync(predicate);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }


        #region Stored Procedures

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedureName, params object[] parameters)
        {
            var sql = parameters.Count() == 0 ? storedProcedureName : BuildStoredProcedureCommand(storedProcedureName, parameters);

            return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }

        public async Task<int> ExecuteStoredProcedureNonQueryAsync(string storedProcedureName, params object[] parameters)
        {
            var sql = parameters.Count() == 0 ? storedProcedureName : BuildStoredProcedureCommand(storedProcedureName, parameters);
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        private static string BuildStoredProcedureCommand(string storedProcedureName, params object[] parameters)
        {
            // Generate SQL for the stored procedure call
            string builder = storedProcedureName;
            foreach (var parameter in (SqlParameter[])parameters)
            {
                builder += $" {parameter.ParameterName},";
            }
            if (builder.EndsWith(",")) builder = builder.Remove(builder.Length - 1);
            return builder;
        }

        public T GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(bool isTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = isTracking ? _dbSet : _dbSet.AsNoTracking();
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query.ToList();
        }

        public IEnumerable<T> GetLastRecords(int count, bool isTracking = true, params Expression<Func<T, object>>[] includes)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<T>();
            }

            IQueryable<T> query = isTracking ? _dbSet : _dbSet.AsNoTracking();
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return OrderByIdDescending(query).Take(count).ToList();
        }

        public IEnumerable<T> Filter(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = isTracking ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query.ToList();
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, bool isTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = isTracking ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);
            if (includes != null)
            {

                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query.FirstOrDefault();
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Any(predicate);
        }

        public T Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
            _context.SaveChanges();
            return entities;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            _context.SaveChanges();
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
        }

        public IEnumerable<T> ExecuteStoredProcedure(string storedProcedureName, params object[] parameters)
        {
            var sql = parameters.Count() == 0 ? storedProcedureName : BuildStoredProcedureCommand(storedProcedureName, parameters);

            return _context.Set<T>().FromSqlRaw(sql, parameters).ToList();
        }



        public int ExecuteStoredProcedureNonQuery(string storedProcedureName, params object[] parameters)
        {
            var sql = parameters.Count() == 0 ? storedProcedureName : BuildStoredProcedureCommand(storedProcedureName, parameters);
            return _context.Database.ExecuteSqlRaw(sql, parameters);
        }

        private IQueryable<T> OrderByIdDescending(IQueryable<T> query)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var idProperty = entityType?.FindProperty("Id");

            if (idProperty == null)
            {
                throw new InvalidOperationException($"{typeof(T).Name} does not have an Id property mapped in the DbContext.");
            }

            var parameter = Expression.Parameter(typeof(T), "entity");
            Expression propertyAccess = idProperty.PropertyInfo != null
                ? Expression.Property(parameter, idProperty.PropertyInfo)
                : Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { idProperty.ClrType },
                    parameter,
                    Expression.Constant(idProperty.Name));

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var orderByMethod = typeof(Queryable)
                .GetMethods()
                .Single(method => method.Name == nameof(Queryable.OrderByDescending)
                    && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), idProperty.ClrType);

            return (IQueryable<T>)orderByMethod.Invoke(null, new object[] { query, orderByExpression });
        }



        #endregion
    }
}
