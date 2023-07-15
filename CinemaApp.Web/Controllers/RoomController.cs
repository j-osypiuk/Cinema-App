using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CinemaApp.Web.Controllers
{
	[Authorize(Roles = SD.Role_Employee)]
    public class RoomController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public RoomController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IActionResult> Index()
		{	
			var rooms = await _unitOfWork.Room.GetAllAsync();

			if (rooms == null)
			{
				return NotFound();
			}

			return View(rooms);
		}

		public async Task<IActionResult> Details(int? id)
		{	
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var room = await _unitOfWork.Room.GetByAsync(x => x.Id == id, includeProperties: "Screenings");
			
			if (room == null)
			{
				return NotFound();
			}

			foreach(var screening in room.Screenings)
			{
				screening.Movie = await _unitOfWork.Movie.GetAsync(screening.MovieId);
			}

			room.Screenings = room.Screenings.Where(x => x.StartTime.Date >= DateTime.Today).OrderBy(x => x.StartTime).ToList();

			return View(room);
		}
	}
}
