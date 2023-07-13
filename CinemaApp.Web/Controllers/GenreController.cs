using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Web.Controllers
{
    [Authorize(Roles = SD.Role_Employee)]
	public class GenreController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        public GenreController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
		{	
			var genres = await _unitOfWork.Genre.GetAllAsync();
			return View(genres);
		}

		public IActionResult Create() 
		{
			return View();
		}

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

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var genre = _unitOfWork.Genre.GetAsync(id).Result;

			if (genre == null)
			{
				return NotFound();
			}

			_unitOfWork.Genre.Remove(genre);
			await _unitOfWork.SaveAsync();

			return RedirectToAction("Index");
		}
	}
}
