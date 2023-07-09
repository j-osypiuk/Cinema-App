using CinemaApp.DataAccess.Data;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Web.Controllers
{
	public class RoomController : Controller
	{
		private readonly ApplicationDbContext _db;
		public RoomController(ApplicationDbContext db)
		{
			_db = db;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int? id)
		{
			var room = _db.Rooms.Where(x => x.Id == id).Include(x => x.Screenings).ThenInclude(x => x.Movie).First();
			var dates = room.Screenings.Where(x => x.StartTime >= DateTime.Today).Select(x => x.StartTime.Date).OrderBy(x => x.Year).ThenBy(x => x.Day).Distinct().ToList();
			
			if (room == null || dates == null)
				return NotFound();
			
			room.Screenings = room.Screenings.OrderBy(x => x.StartTime).ToList();

			var roomVM = new RoomVM
			{
				Room = room,
				ScreeningDates = dates
			};

			return View(roomVM);
		}
	}
}
