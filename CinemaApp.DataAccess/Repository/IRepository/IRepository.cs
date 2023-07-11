using Newtonsoft.Json.Bson;
using System.Linq.Expressions;

namespace CinemaApp.DataAccess.Repository.IRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetAsync(int? id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
    }
}
