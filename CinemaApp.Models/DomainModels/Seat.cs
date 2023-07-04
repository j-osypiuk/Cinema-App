namespace CinemaApp.Models.DomainModels
{
	public class Seat
	{
        public int Id { get; set; }
        public int row { get; set; }
        public int number { get; set; }
        public Room Room { get; set; }
        public int RoomId { get; set; }
    }
}
