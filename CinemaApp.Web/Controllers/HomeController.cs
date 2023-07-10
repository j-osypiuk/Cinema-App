using CinemaApp.DataAccess.Data;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CinemaApp.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		private readonly ApplicationDbContext _db;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
		{
			_logger = logger;
			_db = db;
		}

		public IActionResult Index()
		{	
			var genres = _db.Genres.OrderBy(x => x.Name).ToList();
			var movies = _db.Movies.Include(x => x.MovieGenres).ThenInclude(x => x.Genre).ToList();

			var homeVM = new HomeVM
			{
				Genres = genres,
				Movies = movies,
			};

			return View(homeVM);
		}

		public IActionResult GenreMovies(int? genreId)
		{
			if (genreId == null || genreId == 0)
			{
				return NotFound();
			}

			var genreMovies = (from movie in _db.Movies
							  join movieGenre in _db.MovieGenres 
							  on movie.Id equals movieGenre.MovieId
							  where movieGenre.GenreId == genreId 
							  select movie).Include(x => x.MovieGenres).ThenInclude(x => x.Genre).ToList();

			TempData["GenreName"] = _db.Genres.Find(genreId).Name;

			return View(genreMovies);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}