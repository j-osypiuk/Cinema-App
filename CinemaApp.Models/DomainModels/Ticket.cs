namespace CinemaApp.Models.DomainModels
{
	public class Ticket
	{
        public int Id { get; set; }
        public Screening Screening { get; set; }
        public int ScreeningId { get; set; }
        public int SeatNumber { get; set; }
        public int SeatRow { get; set; }
        public double Price { get; set; }
    }
}
