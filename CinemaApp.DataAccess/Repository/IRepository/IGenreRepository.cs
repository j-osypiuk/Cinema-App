using CinemaApp.Models.DomainModels;

namespace CinemaApp.DataAccess.Repository.IRepository
{
    public interface IGenreRepository : IRepository<Genre>
    {
        void Update(Genre genre);
    }
}
