using CinemaApp.Models.DomainModels;
using System.Linq.Expressions;

namespace CinemaApp.DataAccess.Repository.IRepository
{
	public interface IScreeningRepository : IRepository<Screening>
	{
		void Update(Screening screening);

		Task<IEnumerable<Screening>> GetByDateAndRoomAsync(Expression<Func<Screening, bool>> predicate, string? includeProperties = null);	
	}
}
