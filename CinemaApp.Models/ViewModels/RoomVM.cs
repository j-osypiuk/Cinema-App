using CinemaApp.Models.DomainModels;

namespace CinemaApp.Models.ViewModels
{
	public class RoomVM
	{
        public Room	Room { get; set; }
        public ICollection<DateTime> ScreeningDates { get; set; }
    }
}
