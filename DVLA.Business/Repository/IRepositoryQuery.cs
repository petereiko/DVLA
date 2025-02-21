using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.Repository
{
    public interface IRepositoryQuery<T> where T : class 
    {
        Task<T> GetByIdAsync(long id);
        T GetById(long id);
        Task<IEnumerable<T>> FilterAsync(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Filter(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FilterIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> FilterInclude(Expression<Func<T, bool>> predicate,
                                                          params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllIncludeAsync(params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAllInclude(params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        void Add(T entity);
        Task UpdateAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(long id);
        void Delete(long id);
    }
}
