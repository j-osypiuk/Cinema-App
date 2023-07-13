using CinemaApp.DataAccess.Data;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Web.Controllers
{
	public class TicketController : Controller
	{
		private readonly ApplicationDbContext _db;

		public TicketController(ApplicationDbContext db)
		{
			_db = db;
		}

		public IActionResult Index(int? movieId)
		{	
			if (movieId == null || movieId == 0)
			{
				return NotFound();
			}

<<<<<<< Updated upstream
			var movieScreenings = _db.Screenings.Where(x => x.Movie.Id == movieId && x.StartTime >= DateTime.Now).Include(x => x.Movie).Include(x => x.Room).OrderBy(x => x.StartTime).ToList();
=======
			var movieScreenings = await _unitOfWork.Screening.FindAsync(x => x.Movie.Id == movieId && x.StartTime >= DateTime.Now, includeProperties: "Movie,Room");
			movieScreenings = movieScreenings.OrderBy(x => x.StartTime).ToList();
>>>>>>> Stashed changes

			if (movieScreenings == null)
			{
				return NotFound();
			}

			return View(movieScreenings);
		}

		public IActionResult Create(int? screeningId)
		{
			if (screeningId == null || screeningId == 0)
			{
				return NotFound();
			}

			var screening = _db.Screenings.Where(x => x.Id == screeningId).Include(x => x.Movie).Include(x => x.Tickets).FirstOrDefault();

			var ticketVM = new TicketVM
			{
				Ticket = new Ticket
				{
					Price = screening.Movie.TicketPrice,
					RoomId = screening.RoomId,
					ScreeningId = screening.Id,
				},
				MovieTitle = screening.Movie.Title,
				TicketsBought = screening.Tickets,
				MovieImageUrl = screening.Movie.ImageUrl
			};

			return View(ticketVM);
		}

		[HttpPost]
		public IActionResult Create(TicketVM ticketVM)
		{
			if(ModelState.IsValid)
			{
				if (_db.Tickets.Any(x => x.Row == ticketVM.SelectedRowNumber && x.Number == ticketVM.SelectedSeatNumber && x.ScreeningId == ticketVM.Ticket.ScreeningId))
				{
					ModelState.AddModelError(nameof(ticketVM.SelectedSeatNumber), "This seat is already reserved.");
                    ticketVM.TicketsBought = _db.Tickets.Where(x => x.ScreeningId == ticketVM.Ticket.ScreeningId).ToList();
					return View(ticketVM);
				}

				ticketVM.Ticket.Row = ticketVM.SelectedRowNumber;
				ticketVM.Ticket.Number = ticketVM.SelectedSeatNumber;

				_db.Tickets.Add(ticketVM.Ticket);
				_db.SaveChanges();
				return RedirectToAction("Index", "Home");
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
