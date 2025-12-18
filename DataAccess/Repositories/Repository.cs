using DataAccess.Abstract;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private DbSet<T> dbSet;

        public Repository(MasterContext context)
        {
            dbSet = context.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(Expression<Func<T, bool>> predicate)
        {
            var entities = dbSet.Where(predicate);
            dbSet.RemoveRange(entities);
        }

        public bool Exist(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return dbSet.SingleOrDefault(predicate);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ?
                   dbSet.AsEnumerable() :
                   dbSet.Where(predicate)
                        .AsEnumerable();
        }

        public T GetById(Guid id)
        {
            return dbSet.SingleOrDefault(entity => entity.Id.Equals(id));
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        List<T> IRepository<T>.GetAll(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
            {
                // AsNoTracking() performansı artırır (verileri sadece okumak için çekiyorsan).
                return dbSet.AsNoTracking().ToList();
            }
            else
            {
                // Filtre varsa, o filtreyi uygula ve sonucu getir.
                return dbSet.Where(filter).AsNoTracking().ToList();
            }
        }
    }
}
