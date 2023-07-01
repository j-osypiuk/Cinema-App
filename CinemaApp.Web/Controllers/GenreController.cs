using CinemaApp.DataAccess.Data;
using CinemaApp.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Web.Controllers
{
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
		public async Task<IActionResult> CreateAsync(Genre genre)
		{
			if(ModelState.IsValid)
			{
				_db.Genres.Add(genre);
				await _db.SaveChangesAsync();

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
		public async Task<IActionResult> EditAsync(Genre genre)
		{
			if(ModelState.IsValid)
			{
				_db.Genres.Update(genre);
				await _db.SaveChangesAsync();

				return RedirectToAction("Index");
			}
			return View(genre);
		}

		[ActionName("Delete")]
		public async Task<IActionResult> DeleteAsync(int? id)
		{
			if (id == null || id == 0)
				return NotFound();

			var genre = _db.Genres.Find(id);

			if(genre == null)
				return NotFound();

			_db.Genres.Remove(genre);
			await _db.SaveChangesAsync();

			return RedirectToAction("Index");
		}
	}
}
