using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaApp.Models.DomainModels
{
	public class Ticket
	{
		public int Id { get; set; }
		public Screening? Screening { get; set; }
		public int ScreeningId { get; set; }
		public double Price { get; set; }
		public Seat? Seat { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }
        public int RoomId { get; set; }
    }
}
