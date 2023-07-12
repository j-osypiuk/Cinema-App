using Newtonsoft.Json.Bson;
using System.Linq.Expressions;

namespace CinemaApp.DataAccess.Repository.IRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetAsync(int? id);
        Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> predicate, string? includeProperties = null);
        Task<IEnumerable<TEntity>> GetAllAsync(string? includeProperties = null);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string? includeProperties = null);
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
    }
}
