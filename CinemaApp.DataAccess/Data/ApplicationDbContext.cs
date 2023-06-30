using CinemaApp.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.DataAccess.Data
{
	public class ApplicationDbContext : DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MovieGenre>()
				.HasKey(x => new {x.MovieId, x.GenreId});
		}
	}
}
