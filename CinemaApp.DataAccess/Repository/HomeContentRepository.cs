using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.DataAccess.Repository
{
	public class HomeContentRepository : Repository<HomeContent>, IHomeContentRepository
	{
        public HomeContentRepository(ApplicationDbContext db) : base(db) { }

		public async Task<IEnumerable<Movie>> GetGenreMoviesAsync(int? genreId)
		{
			var genreMovies = (from movie in _db.Movies
							   join movieGenre in _db.MovieGenres
							   on movie.Id equals movieGenre.MovieId
							   where movieGenre.GenreId == genreId
							   select movie).Include(x => x.MovieGenres).ThenInclude(x => x.Genre);

			return await genreMovies.ToListAsync(); 
		}

		public void Update(HomeContent homeContent)
		{
			_dbSet.Update(homeContent);
		}
	}
}
