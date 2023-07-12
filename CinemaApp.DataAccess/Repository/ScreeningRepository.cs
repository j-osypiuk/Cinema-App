using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaApp.DataAccess.Repository
{
	public class ScreeningRepository : Repository<Screening>, IScreeningRepository
	{
        public ScreeningRepository(ApplicationDbContext db) : base(db) { }

		public async Task<IEnumerable<Screening>> GetByDateAndRoomAsync(Expression<Func<Screening, bool>> predicate, string? includeProperties = null)
		{
			IQueryable<Screening> query = _dbSet;

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			query = IncludeProperties(query, includeProperties);
			return await query.ToListAsync();
		}

		public void Update(Screening screening)
		{
			_dbSet.Update(screening);
		}
	}
}
