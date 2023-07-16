using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using CinemaApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Web.Controllers
{
	public class MovieController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public MovieController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
            _unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		[Authorize(Roles = SD.Role_Employee)]
		public async Task<IActionResult> Index()
		{
			var movieGenres = await _unitOfWork.MovieGenre.GetAllAsync(includeProperties: "Movie,Genre");
			var movies = movieGenres.Select(x => x.Movie).Distinct().ToList();

			return View(movies);
		}

		[Authorize(Roles = SD.Role_Employee)]
		public async Task<IActionResult> Create()
		{
			var genres = await _unitOfWork.Genre.GetAllAsync();
			MovieVM movieVM = new MovieVM
			{
				Genres = genres,
				Movie = new Movie()
			};
			movieVM.Movie.ReleaseDate = DateTime.Now;
			return View(movieVM);
		}

		[Authorize(Roles = SD.Role_Employee)]
		[HttpPost]
		public async Task<IActionResult> Create(MovieVM movieVM, IFormFile? formFile)
		{
			if (ModelState.IsValid)
			{
				if (movieVM.SelectedGenreIds.Count <= 0)
				{
					ModelState.AddModelError("Movie.MovieGenres", "Movie must belongs to at least one genre.");
					movieVM.Genres = await _unitOfWork.Genre.GetAllAsync();
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
				} else
				{
					movieVM.Movie.ImageUrl = @"\images\default\default-movie-poster.jpg";
				}

				movieVM.Movie.MovieGenres = movieVM.SelectedGenreIds.Select(id => new MovieGenre { GenreId = id }).ToList();

				await _unitOfWork.Movie.AddAsync(movieVM.Movie);
				await _unitOfWork.SaveAsync();

				return RedirectToAction("Index");
			}
			movieVM.Genres = await _unitOfWork.Genre.GetAllAsync();
			return View(movieVM);
		}

		public async Task<IActionResult> Details(int? id)
		{
			var movieGenres = await _unitOfWork.MovieGenre.FindAsync(x => x.MovieId == id, includeProperties: "Movie,Genre");
			var movie = movieGenres.Select(x => x.Movie).FirstOrDefault();

			return View(movie);
		}

		[Authorize(Roles = SD.Role_Employee)]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var movieGenres = await _unitOfWork.MovieGenre.FindAsync(x => x.MovieId == id, includeProperties: "Movie,Genre");
			var movie = movieGenres.Select(x => x.Movie).FirstOrDefault();

			if (movie == null)
			{
				return NotFound();
			}

			MovieVM movieVM = new MovieVM
			{
				Movie = movie,
				SelectedGenreIds = movie.MovieGenres.Select(x => x.GenreId).ToList(),
				Genres = await _unitOfWork.Genre.GetAllAsync()
			};

			return View(movieVM);
		}

		[Authorize(Roles = SD.Role_Employee)]
		[HttpPost]
		public async Task<IActionResult> Edit(MovieVM movieVM, IFormFile? formFile)
		{
			if (ModelState.IsValid)
			{
				if (movieVM.SelectedGenreIds.Count <= 0)
				{
					ModelState.AddModelError("Movie.MovieGenres", "Movie must belongs to at least one genre.");
					movieVM.Genres = await _unitOfWork.Genre.GetAllAsync();
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

						if (System.IO.File.Exists(oldImagePath) && !oldImagePath.Contains("default"))
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

				movieVM.Movie.MovieGenres = (ICollection<MovieGenre>) await _unitOfWork.MovieGenre.FindAsync(x => x.MovieId == movieVM.Movie.Id);

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
						_unitOfWork.MovieGenre.Remove(movieGenre);
					}
				}

				_unitOfWork.Movie.Update(movieVM.Movie);
				await _unitOfWork.SaveAsync();

				return RedirectToAction("Index");
			}
			movieVM.Genres = await _unitOfWork.Genre.GetAllAsync();
			return View(movieVM);
		}

		[Authorize(Roles = SD.Role_Employee)]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var movie = await _unitOfWork.Movie.GetAsync(id);

			if (movie == null)
			{
				return NotFound();
			}

			if (!string.IsNullOrEmpty(movie.ImageUrl))
			{
				var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, movie.ImageUrl.TrimStart('\\'));

				if (System.IO.File.Exists(oldImagePath) && !oldImagePath.Contains("default"))
				{
					System.IO.File.Delete(oldImagePath);
				}
			}

			_unitOfWork.Movie.Remove(movie);
			await _unitOfWork.SaveAsync();

			return View();
		}
	}
}
