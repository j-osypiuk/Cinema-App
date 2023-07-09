using CinemaApp.DataAccess.Data;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CinemaApp.Web.Controllers
{
	public class ScreeningController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IEnumerable<SelectListItem> _selectedMovieList;

		public ScreeningController(ApplicationDbContext db)
		{
			_db = db;
			_selectedMovieList = _db.Movies.Select(x => new SelectListItem
			{
				Text = x.Title,
				Value = x.Id.ToString()
			});
		}

		public IActionResult Index()
		{	
			var screenings = _db.Screenings.Include(x => x.Movie).Include(x => x.Room).ToList();
			return View(screenings);
		}


		public IActionResult Create(int? roomId) 
		{	
			if(roomId == null || roomId == 0)
			{
				return NotFound();
			}

			var screeningVM = new ScreeningVM
			{
				Screening = new Screening
				{
					RoomId = (int)roomId,
					StartTime = DateTime.Today
				},
				MovieSelectList = _selectedMovieList,
			};
			return View(screeningVM);
		}

		[HttpPost]
		public IActionResult Create(ScreeningVM screeningVM)
		{
			if(ModelState.IsValid)
			{
				var movie = _db.Movies.Find(screeningVM.SelectedMovieId);
				var screenings = _db.Screenings
									.Where(x => x.StartTime.Date == screeningVM.Screening.StartTime.Date && x.RoomId == screeningVM.Screening.RoomId)
									.Include(x => x.Movie).ToList();


				if (movie == null)
				{
					screeningVM.MovieSelectList = _selectedMovieList;
					if (movie == null)
					{
						ModelState.AddModelError(nameof(screeningVM.SelectedMovieId), "Selected Movie does not exitst.");
					}
					return View(screeningVM);
				}

				// Checks if there is no time conflict with other movie screenings
				bool conflict = false;
				foreach (var screening in screenings)
				{
					if (screeningVM.Screening.StartTime >= screening.StartTime
						&& screeningVM.Screening.StartTime <= screening.StartTime.AddMinutes(screening.Movie.Duration))
					{
						conflict = true;
					}
					if (screeningVM.Screening.StartTime.AddMinutes(movie.Duration) >= screening.StartTime
						&& screeningVM.Screening.StartTime.AddMinutes(movie.Duration) <= screening.StartTime.AddMinutes(screening.Movie.Duration))
					{
						conflict = true;
					}
					if (screening.StartTime >= screeningVM.Screening.StartTime
						&& screening.StartTime.AddMinutes(screening.Movie.Duration) <= screeningVM.Screening.StartTime.AddMinutes(movie.Duration))
					{
						conflict = true;
					}
				}

				if (conflict)
				{
					ModelState.AddModelError("Screening.StartTime", "Another movie is shown at that time.");
					screeningVM.MovieSelectList = _selectedMovieList;
					return View(screeningVM);
				}

				// Checks if screening date is at least tomorrow
				if (screeningVM.Screening.StartTime.Date <= DateTime.Today)
				{
					ModelState.AddModelError("Screening.StartTime", "Screening date must be at least tomorrow.");
					screeningVM.MovieSelectList = _selectedMovieList;
					return View(screeningVM);
				}

				Screening newScreening = new Screening
				{
					StartTime = screeningVM.Screening.StartTime,
					Is3D = screeningVM.Screening.Is3D,
					Movie = movie,
					RoomId = screeningVM.Screening.RoomId
				};

				_db.Screenings.Add(newScreening);
				_db.SaveChanges();
				return RedirectToAction("Details", "Room", new { id = screeningVM.Screening.RoomId });
			}
			return View();
		}

		public IActionResult Details(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var screening = _db.Screenings.Where(x => x.Id == id).Include(x => x.Movie).Include(x => x.Room).Include(x => x.Tickets).FirstOrDefault();
			
			if (screening == null)
			{
				return NotFound();
			}

			return View(screening);
		}

		public IActionResult Delete(int? id)
		{	
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var screening = _db.Screenings.Where(x => x.Id == id).Include(x => x.Tickets).FirstOrDefault();

			if (screening == null)
			{
				return NotFound();
			}

			_db.Screenings.Remove(screening);
			_db.SaveChanges();

			return RedirectToAction("Details", "Room", new {id = screening.RoomId});
		}
	}
}
