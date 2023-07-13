using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApp.DataAccess.Repository
{
    public class MovieGenreRepository : Repository<MovieGenre>, IMovieGenreRepository
    {
        public MovieGenreRepository(ApplicationDbContext db) : base(db) { }

        public void Update(MovieGenre movieGenre)
        {
            _dbSet.Update(movieGenre);
        }
    }
}
