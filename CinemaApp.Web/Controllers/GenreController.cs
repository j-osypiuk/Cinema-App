using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Web.Controllers
{

	public class GenreController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		[Authorize(Roles = SD.Role_Employee)]
		public async Task<IActionResult> Index()
		{	
			var genres = await _unitOfWork.Genre.GetAllAsync();
			return View(genres);
		}

		[Authorize(Roles = SD.Role_Employee)]
		public IActionResult Create() 
		{
			return View();
		}

		public async Task<IActionResult> GenreMovies(int? genreId)
		{
			if (genreId == null || genreId == 0)
			{
				return NotFound();
			}

			var movieGenres = await _unitOfWork.MovieGenre.GetAllAsync(includeProperties: "Movie,Genre");
			var genre = movieGenres.Where(x => x.GenreId == genreId).Select(x => x.Genre).FirstOrDefault();
			var genreMovies = movieGenres.Where(x => x.GenreId == genreId).Select(x => x.Movie).ToList();

			TempData["GenreName"] = genre.Name;

			return View(genreMovies);
		}

		[Authorize(Roles = SD.Role_Employee)]
		[HttpPost]
		public async Task<IActionResult> Create(Genre genre)
		{
			if(ModelState.IsValid)
			{
				await _unitOfWork.Genre.AddAsync(genre);
				await _unitOfWork.SaveAsync();

				return RedirectToAction("Index");
			}
			return View();
		}

		[Authorize(Roles = SD.Role_Employee)]
		public async Task<IActionResult> Edit(int? id)
		{	
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var genre = await _unitOfWork.Genre.GetAsync(id);

			if (genre == null)
			{
				return NotFound();
			}
			return View(genre);
		}

		[Authorize(Roles = SD.Role_Employee)]
		[HttpPost]
		public async Task<IActionResult> Edit(Genre genre)
		{
			if(ModelState.IsValid)
			{
				_unitOfWork.Genre.Update(genre);
				await _unitOfWork.SaveAsync();

				return RedirectToAction("Index");
			}
			return View(genre);
		}

		[Authorize(Roles = SD.Role_Employee)]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var genre = await _unitOfWork.Genre.GetAsync(id);

			if (genre == null)
			{
				return NotFound();
			}

			_unitOfWork.Genre.Remove(genre);
			await _unitOfWork.SaveAsync();

			return View();
		}
	}
}
