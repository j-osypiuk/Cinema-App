using CinemaApp.Models.DomainModels;

namespace CinemaApp.DataAccess.Repository.IRepository
{
	public interface IHomeContentRepository : IRepository<HomeContent>
	{
		void Update(HomeContent homeContent);
		Task<IEnumerable<Movie>> GetGenreMoviesAsync(int? genreId);
	}
}
