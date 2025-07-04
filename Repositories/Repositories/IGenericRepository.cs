using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{

    public interface IGenericRepository<T> where T : class
    {
        //queryable
        IQueryable<T> Entities { get; }

        //void
        T? GetById(object id);
        void Insert(T obj);
        void InsertRange(List<T> obj);
        Task InsertRangeAsync(List<T> obj);

        void Update(T obj);
        void Delete(object entity);
        void Save();

        //Task
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task InsertAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(object entity);
        Task SaveAsync();


        //another
        T? Find(Expression<Func<T, bool>> predicate);


    }


}
