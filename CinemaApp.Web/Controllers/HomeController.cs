using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Specialized;
using System.Diagnostics;

namespace CinemaApp.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		private readonly IWebHostEnvironment _webHostEnvironment;
		public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
		{
			var genres = await _unitOfWork.Genre.GetAllAsync();
			var movies =  await _unitOfWork.Movie.GetAllAsync(includeProperties: "MovieGenres");
			foreach (var movie in movies)
			{
				foreach (var movieGenre in movie.MovieGenres)
				{
					movieGenre.Genre = await _unitOfWork.Genre.GetAsync(movieGenre.GenreId);
				}
			}
			var homeContent = await _unitOfWork.HomeContent.GetAllAsync();

			var homeVM = new HomeVM
			{	
				HomeContent = homeContent.FirstOrDefault(),
				Genres = genres.OrderBy(x => x.Name),
				Movies = movies,
			};

			return View(homeVM);
		}

		public async Task<IActionResult> GenreMovies(int? genreId)
		{
			if (genreId == null || genreId == 0)
			{
				return NotFound();
			}

			var genre = await _unitOfWork.Genre.GetAsync(genreId);
			var genreMovies = await _unitOfWork.HomeContent.GetGenreMoviesAsync(genreId);

			TempData["GenreName"] = genre.Name;

			return View(genreMovies);
		}

		public async Task<IActionResult> Edit()
		{
			var homeContent = await _unitOfWork.HomeContent.GetAllAsync();

			if (homeContent == null)
			{
				return NotFound();
			}

			return View(homeContent.FirstOrDefault());
		}

		[HttpPost] 
		public async Task<IActionResult> Edit(HomeContent? homeContent, IFormFile? CarouselImg_1, IFormFile? CarouselImg_2, IFormFile? CarouselImg_3)
		{
			var carouselImgs = new List<IFormFile> { CarouselImg_1, CarouselImg_2, CarouselImg_3 };

			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				for (var i = 0; i < carouselImgs.Count; i++)
				{
					if (carouselImgs[i] != null)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(carouselImgs[i].FileName);
						string carouselImgPath = Path.Combine(wwwRootPath, @"images\carousel\");
						string oldImagePath = string.Empty;

						using (var fileStream = new FileStream(Path.Combine(carouselImgPath, fileName), FileMode.Create))
						{
							carouselImgs[i].CopyTo(fileStream);
						}

						// Choose which current carousel img url is changing and old img file will be deleted
						switch (i)
						{
							case 0:
								if (!string.IsNullOrEmpty(homeContent.CarouselImgUrl_1))
								{
									oldImagePath = Path.Combine(wwwRootPath, homeContent.CarouselImgUrl_1.TrimStart('\\'));
								}
								homeContent.CarouselImgUrl_1 = @"\images\carousel\" + fileName;
								break;
							case 1:
								if (!string.IsNullOrEmpty(homeContent.CarouselImgUrl_2))
								{
									oldImagePath = Path.Combine(wwwRootPath, homeContent.CarouselImgUrl_2.TrimStart('\\'));
								}
								homeContent.CarouselImgUrl_2 = @"\images\carousel\" + fileName;
								break;
							case 2:
								if (!string.IsNullOrEmpty(homeContent.CarouselImgUrl_3))
								{
									oldImagePath = Path.Combine(wwwRootPath, homeContent.CarouselImgUrl_3.TrimStart('\\'));
								}
								homeContent.CarouselImgUrl_3 = @"\images\carousel\" + fileName;
								break;
						}

						// Delete old carousel img file
						if (System.IO.File.Exists(oldImagePath) && !oldImagePath.Contains("default"))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}
				}
				_unitOfWork.HomeContent.Update(homeContent);
				await _unitOfWork.SaveAsync();

				return RedirectToAction("Index", "Home");
			}
			return View(homeContent);
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