namespace CinemaApp.Models.DomainModels
{
	public class Seat
	{
		public int Row { get; set; }
		public int Number  { get; set; }
		public Room Room { get; set; }
		public int RoomId { get; set; }
		public ICollection<Ticket>? Tickets { get; set; }
	}
}