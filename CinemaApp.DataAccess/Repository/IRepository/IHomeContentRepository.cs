using CinemaApp.Models.DomainModels;

namespace CinemaApp.DataAccess.Repository.IRepository
{
	public interface IHomeContentRepository : IRepository<HomeContent>
	{
		void Update(HomeContent homeContent);
	}
}
