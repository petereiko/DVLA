using DVLA.Data;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.Repository
{
    public class GenericRepository<T>: IRepositoryQuery<T> where T : class
    {
        private readonly DVLADbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DVLADbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public T GetById(long id)
        {
            return _dbSet.Find(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IEnumerable<T>> GetLastRecordsAsync(int count)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<T>();
            }

            return await OrderByIdDescending(_dbSet).Take(count).ToListAsync();
        }

        public IEnumerable<T> GetLastRecords(int count)
        {
            if (count <= 0)
            {
                return Enumerable.Empty<T>();
            }

            return OrderByIdDescending(_dbSet).Take(count).ToList();
        }

        public async Task<IEnumerable<T>> GetAllIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public IEnumerable<T> GetAllInclude(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return  query.ToList();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public IEnumerable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public async Task<IEnumerable<T>> FilterIncludeAsync(Expression<Func<T, bool>> predicate,
                                                          params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public IEnumerable<T> FilterInclude(Expression<Func<T, bool>> predicate,
                                                          params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.Where(predicate).ToList();
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

    }
}
