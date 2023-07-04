namespace CinemaApp.Models.DomainModels
{
	public class Room
	{
        public int Id { get; set; }
        public int number { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Screening> Screenings { get; set; }
    }
}
