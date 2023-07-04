namespace CinemaApp.Models.DomainModels
{
	public class Screening
	{
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public bool Is3D { get; set; }
        public Movie Movie { get; set; }
        public int MovieId { get; set; }
        public Room Room { get; set; }
        public int RoomId { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
