using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;

namespace CinemaApp.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IGenreRepository Genre { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {   
            _db = db;
            Genre = new GenreRepository(db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
