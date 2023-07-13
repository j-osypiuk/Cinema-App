using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace CinemaApp.DataAccess.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected ApplicationDbContext _db { get; }
        internal DbSet<TEntity> _dbSet { get; }
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = IncludeProperties(query, includeProperties);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;
            
            query = IncludeProperties(query, includeProperties);

            return await query.ToListAsync();
        }

		public async Task<TEntity?> GetAsync(int? id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);   
        }

        public async Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = IncludeProperties(query, includeProperties);

            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<TEntity> IncludeProperties(IQueryable<TEntity> query, string? properties) 
        {
            if (!string.IsNullOrEmpty(properties))
            {
                foreach (var property in properties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query;
        }
    }
}
