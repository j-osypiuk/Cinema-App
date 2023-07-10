using CinemaApp.Models.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.DataAccess.Data
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<Movie> Movies { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<MovieGenre> MovieGenres { get; set; }
		public DbSet<Screening> Screenings { get; set; }
		public DbSet<Room> Rooms { get; set; }
		public DbSet<Seat> Seats { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{	
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Movie>(eb =>
			{
				eb.Property(x => x.Duration).IsRequired();
				eb.Property(x => x.TicketPrice).IsRequired();
				eb.HasMany(x => x.Screenings).WithOne(x => x.Movie).HasForeignKey(x => x.MovieId);
			});

			modelBuilder.Entity<Room>(eb =>
			{
				eb.Property(x => x.Number).IsRequired();
				eb.HasIndex(x => x.Number).IsUnique();
				eb.HasMany(x => x.Seats).WithOne(x => x.Room).HasForeignKey(x => x.RoomId);
				eb.HasMany(x => x.Screenings).WithOne(x => x.Room).HasForeignKey(x => x.RoomId);
			});

			modelBuilder.Entity<MovieGenre>(eb =>
			{
				eb.HasKey(x => new { x.MovieId, x.GenreId });
			});

			modelBuilder.Entity<Screening>(eb =>
			{
				eb.Property(x => x.Is3D).IsRequired();
				eb.HasMany(x => x.Tickets).WithOne(x => x.Screening).HasForeignKey(x => x.ScreeningId);
			});

			modelBuilder.Entity<Seat>(eb =>
			{
				eb.HasKey(x => new { x.Row, x.Number, x.RoomId });
				eb.HasMany(x => x.Tickets)
				.WithOne(x => x.Seat)
				.HasForeignKey(x => new { x.Row, x.Number, x.RoomId })
				.OnDelete(DeleteBehavior.NoAction);
			});
		}
	}
}