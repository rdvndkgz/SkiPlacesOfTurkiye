using Entities;
using System.Linq.Expressions;

namespace DataAccess.Abstract
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T entity);
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        T GetById(Guid id);
        T Get(Expression<Func<T, bool>> predicate);
        bool Exist(Expression<Func<T, bool>> predicate);
        void Delete(T entity);
        void DeleteRange(Expression<Func<T, bool>> predicate);
        void Update(T entity);
    }
}
