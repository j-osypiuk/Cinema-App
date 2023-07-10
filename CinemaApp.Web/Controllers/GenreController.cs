using CinemaApp.DataAccess.Data;
using CinemaApp.Models.DomainModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace CinemaApp.Web.Controllers
{
	[Authorize(Roles = SD.Role_Employee)]
	public class GenreController : Controller
	{
		private readonly ApplicationDbContext _db;

        public GenreController(ApplicationDbContext db)
        {
			_db = db;
        }

        public IActionResult Index()
		{	
			var genres = _db.Genres.ToList();
			return View(genres);
		}

		public IActionResult Create() 
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Genre genre)
		{
			if(ModelState.IsValid)
			{
				_db.Genres.Add(genre);
				_db.SaveChanges();

				return RedirectToAction("Index");
			}
			return View();
		}

		public IActionResult Edit(int? id)
		{	
			if(id == null || id == 0)
				return NotFound();

			var genre = _db.Genres.Find(id);

			if(genre == null) 
				return NotFound();

			return View(genre);
		}

		[HttpPost]
		public IActionResult Edit(Genre genre)
		{
			if(ModelState.IsValid)
			{
				_db.Genres.Update(genre);
				_db.SaveChanges();

				return RedirectToAction("Index");
			}
			return View(genre);
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var genre = _db.Genres.Find(id);

			if(genre == null)
				return NotFound();

			_db.Genres.Remove(genre);
			_db.SaveChanges();

			return RedirectToAction("Index");
		}
	}
}
