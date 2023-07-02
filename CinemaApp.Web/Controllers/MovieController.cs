using CinemaApp.DataAccess.Data;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Web.Controllers
{
	public class MovieController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public MovieController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
		{
			_db = db;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
		{	
			var movies = _db.Movies.Include(x => x.MovieGenres).ThenInclude(x => x.Genre).AsNoTracking().ToList();
			return View(movies);
		}

		public IActionResult Create()
		{	
			var genres = _db.Genres.ToList();
			MovieVM movieVM = new MovieVM
			{
				Genres = genres,
				Movie = new Movie()
			};
			movieVM.Movie.ReleaseDate = DateTime.Now;
			return View(movieVM);
		}

		[HttpPost]
		public IActionResult Create(MovieVM movieVM, IFormFile? formFile)
		{
			if (ModelState.IsValid)
			{	
				if (movieVM.SelectedGenreIds.Count <= 0)
				{
					ModelState.AddModelError("Movie.MovieGenres", "Movie must belongs to at least one genre.");
					movieVM.Genres = _db.Genres.ToList();
					return View(movieVM);
				}

				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (formFile != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
					string moviePath = Path.Combine(wwwRootPath, @"images\movie\");

					using (var fileStream = new FileStream(Path.Combine(moviePath, fileName), FileMode.Create))
					{
						formFile.CopyTo(fileStream);
					}
					movieVM.Movie.ImageUrl = @"\images\movie\" + fileName;
				}

				movieVM.Movie.MovieGenres = movieVM.SelectedGenreIds.Select(id => new MovieGenre { GenreId = id }).ToList();

				_db.Movies.Add(movieVM.Movie);
				_db.SaveChanges();	

				return RedirectToAction("Index");
			}
			return View(movieVM);
		}

		public IActionResult Details(int? id)
		{
			var movie = _db.Movies.Where(x => x.Id == id).Include(x => x.MovieGenres).ThenInclude(x => x.Genre).First();
			return View(movie);
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var movie = _db.Movies.Where(x => x.Id == id).Include(x => x.MovieGenres).ThenInclude(x => x.Genre).First();

			if (movie == null) 
				return NotFound();
			
			MovieVM movieVM = new MovieVM
			{
				Movie = movie,
				SelectedGenreIds = movie.MovieGenres.Select(x => x.GenreId).ToList(),
				Genres = _db.Genres.ToList()
			};

			return View(movieVM);
		}

		[HttpPost]
		public IActionResult Edit(MovieVM movieVM, IFormFile? formFile)
		{
			if (ModelState.IsValid)
			{
				if (movieVM.SelectedGenreIds.Count <= 0)
				{
					ModelState.AddModelError("Movie.MovieGenres", "Movie must belongs to at least one genre.");
					movieVM.Genres = _db.Genres.ToList();
					return View(movieVM);
				}

				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (formFile != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
					string moviePath = Path.Combine(wwwRootPath, @"images\movie\");

					if (!string.IsNullOrEmpty(movieVM.Movie.ImageUrl))
					{
						var oldImagePath = Path.Combine(wwwRootPath, movieVM.Movie.ImageUrl.TrimStart('\\'));

						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}

					}
					using (var fileStream = new FileStream(Path.Combine(moviePath, fileName), FileMode.Create))
					{
						formFile.CopyTo(fileStream);
					}
					movieVM.Movie.ImageUrl = @"\images\movie\" + fileName;
				}

				movieVM.Movie.MovieGenres = _db.MovieGenres.Where(x => x.MovieId == movieVM.Movie.Id).ToList();

				foreach (var genreId in movieVM.SelectedGenreIds)
				{
					if (!movieVM.Movie.MovieGenres.Any(x => x.GenreId == genreId))
					{
						movieVM.Movie.MovieGenres.Add(new MovieGenre { GenreId = genreId });
					}
				}

				foreach (var movieGenre in movieVM.Movie.MovieGenres)
				{
					if (!movieVM.SelectedGenreIds.Contains(movieGenre.GenreId))
					{
						_db.MovieGenres.Remove(movieGenre);
					}
				}

				_db.Movies.Update(movieVM.Movie);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View();
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var movie = _db.Movies.Find(id);

			if (movie == null)
				return NotFound();

			if(!string.IsNullOrEmpty(movie.ImageUrl))
			{
				var fileToDeletePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.ImageUrl.TrimStart('\\'));

				if (System.IO.File.Exists(fileToDeletePath))
					System.IO.File.Delete(fileToDeletePath);
			}

			_db.Movies.Remove(movie);
			_db.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}
