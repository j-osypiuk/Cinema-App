namespace CinemaApp.Models.DomainModels
{
	public class Movie
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime Date { get; set;}
        public double Duration { get; set; }
        public ICollection<MovieGenre> MovieGenres { get; set; }
        public string Stars { get; set; }
        public string Director { get; set; }
        public double TicketPrice { get; set; }
        public string? ImageUrl { get; set; }
    }
}
