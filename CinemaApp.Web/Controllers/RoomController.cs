using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Utility;

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
			return View(rooms);
		}

		public async Task<IActionResult> Details(int? id)
		{
			var room = await _unitOfWork.Room.GetByAsync(x => x.Id == id, includeProperties: "Screenings");
			
			foreach(var screening in room.Screenings)
			{
				screening.Movie = await _unitOfWork.Movie.GetAsync(screening.MovieId);
			}

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
