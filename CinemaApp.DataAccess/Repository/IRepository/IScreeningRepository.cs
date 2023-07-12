using CinemaApp.Models.DomainModels;
using System.Linq.Expressions;

namespace CinemaApp.DataAccess.Repository.IRepository
{
	public interface IScreeningRepository : IRepository<Screening>
	{
		void Update(Screening screening);
	}
}
