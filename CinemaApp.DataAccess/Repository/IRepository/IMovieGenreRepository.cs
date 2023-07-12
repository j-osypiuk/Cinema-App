using CinemaApp.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaApp.DataAccess.Repository.IRepository
{
    public interface IMovieGenreRepository : IRepository<MovieGenre>
    {
        void Update(MovieGenre movieGenre);
    }
}
