using CinemaApp.DataAccess.Data;
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
		private readonly ILogger<HomeController> _logger;

		private readonly ApplicationDbContext _db;

		private readonly IWebHostEnvironment _webHostEnvironment;
		public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
		{
			_logger = logger;
			_db = db;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{	
			var genres = _db.Genres.OrderBy(x => x.Name).ToList();
			var movies = _db.Movies.Include(x => x.MovieGenres).ThenInclude(x => x.Genre).ToList();
			var homeContent = _db.HomeContents.FirstOrDefault();

			var homeVM = new HomeVM
			{	
				HomeContent = homeContent,
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

		public IActionResult Edit()
		{	
			var homeContent = _db.HomeContents.FirstOrDefault();

			if (homeContent == null)
			{
				return NotFound();
			}

			return View(homeContent);
		}

		[HttpPost] 
		public IActionResult Edit(HomeContent? homeContent, IFormFile? CarouselImg_1, IFormFile? CarouselImg_2, IFormFile? CarouselImg_3)
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
				_db.HomeContents.Update(homeContent);
				_db.SaveChanges();

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