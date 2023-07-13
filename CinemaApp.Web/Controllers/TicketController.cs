using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Web.Controllers
{
	public class TicketController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public TicketController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IActionResult> Index(int? movieId)
		{	
			if (movieId == null || movieId == 0)
			{
				return NotFound();
			}

<<<<<<< HEAD
<<<<<<< Updated upstream
			var movieScreenings = _db.Screenings.Where(x => x.Movie.Id == movieId && x.StartTime >= DateTime.Now).Include(x => x.Movie).Include(x => x.Room).OrderBy(x => x.StartTime).ToList();
=======
			var movieScreenings = await _unitOfWork.Screening.FindAsync(x => x.Movie.Id == movieId && x.StartTime >= DateTime.Now, includeProperties: "Movie,Room");
			movieScreenings = movieScreenings.OrderBy(x => x.StartTime).ToList();
>>>>>>> Stashed changes
=======
			var movieScreenings = await _unitOfWork.Screening.FindAsync(x => x.Movie.Id == movieId && x.StartTime >= DateTime.Now, includeProperties: "Movie,Room");
			movieScreenings.OrderBy(x => x.StartTime).ToList();
>>>>>>> dee6abe1db6fc32240bec255b3b61b69775eb88e

			if (movieScreenings == null)
			{
				return NotFound();
			}

			return View(movieScreenings);
		}

		public async Task<IActionResult> Create(int? screeningId)
		{
			if (screeningId == null || screeningId == 0)
			{
				return NotFound();
			}

			var screening = await _unitOfWork.Screening.GetByAsync(x => x.Id == screeningId, includeProperties: "Movie,Tickets");

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
		public async Task<IActionResult> Create(TicketVM ticketVM)
		{
			var tickets = await _unitOfWork.Ticket.GetAllAsync();

			if(ModelState.IsValid)
			{
				if (tickets.Any(x => x.Row == ticketVM.SelectedRowNumber && x.Number == ticketVM.SelectedSeatNumber && x.ScreeningId == ticketVM.Ticket.ScreeningId))
				{
					ModelState.AddModelError(nameof(ticketVM.SelectedSeatNumber), "This seat is already reserved.");
					ticketVM.TicketsBought = (ICollection<Ticket>?) await _unitOfWork.Ticket.FindAsync(x => x.ScreeningId == ticketVM.Ticket.ScreeningId);
					return View(ticketVM);
				}

				ticketVM.Ticket.Row = ticketVM.SelectedRowNumber;
				ticketVM.Ticket.Number = ticketVM.SelectedSeatNumber;

				await _unitOfWork.Ticket.AddAsync(ticketVM.Ticket);
				await _unitOfWork.SaveAsync();

				return RedirectToAction("Index", "Home");
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
