namespace CinemaApp.Models.DomainModels
{
	public class SeatReservation
	{
		public int Id { get; set; }
		public Screening Screening { get; set; }
		public int ScreeningId { get; set; }
		public Seat Seat { get; set; }
		public int row { get; set; }
		public int number { get; set; }
		public Ticket Ticket { get; set; }
		public int TicketId { get; set; }
	}
}