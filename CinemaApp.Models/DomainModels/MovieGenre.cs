namespace CinemaApp.Models.DomainModels
{
	public class MovieGenre
	{
        public Genre Genre { get; set; }
        public int GenreId { get; set; }
        public Movie Movie { get; set; }
        public int MovieId { get; set; }
    }
}
