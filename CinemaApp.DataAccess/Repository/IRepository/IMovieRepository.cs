using CinemaApp.Models.DomainModels;

namespace CinemaApp.DataAccess.Repository.IRepository
{
    public interface IMovieRepository : IRepository<Movie>
    {
        void Update(Movie movie);
    }
}
