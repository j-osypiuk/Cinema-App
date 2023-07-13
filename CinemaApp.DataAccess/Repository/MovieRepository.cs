using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.DataAccess.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext db) : base(db) { }
        
        public void Update(Movie movie)
        {
            _dbSet.Update(movie);
        }
    }
}
