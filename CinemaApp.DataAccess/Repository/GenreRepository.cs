using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;

namespace CinemaApp.DataAccess.Repository
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(ApplicationDbContext db) : base(db) { }
        
        public void Update(Genre genre)
        {
            _dbSet.Update(genre);
        }
    }
}
