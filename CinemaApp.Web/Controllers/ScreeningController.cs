﻿using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
<<<<<<< HEAD
<<<<<<< Updated upstream
using System.Runtime.CompilerServices;
=======
>>>>>>> dee6abe1db6fc32240bec255b3b61b69775eb88e
using Utility;
=======
using CinemaApp.Utility;
>>>>>>> Stashed changes

namespace CinemaApp.Web.Controllers
{
	//[Authorize(Roles = SD.Role_Employee)]
    public class ScreeningController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEnumerable<SelectListItem> _selectedMovieList;

		public ScreeningController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_selectedMovieList = _unitOfWork.Movie.GetAllAsync().Result.Select(x => new SelectListItem
			{
				Text = x.Title,
				Value = x.Id.ToString()
			});
		}

		public async Task<IActionResult> Index()
		{
			var screenings = await _unitOfWork.Screening.GetAllAsync(includeProperties: "Movie,Room");
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
		public async Task<IActionResult> Create(ScreeningVM screeningVM)
		{
			if(ModelState.IsValid)
			{
				var movie = await _unitOfWork.Movie.GetAsync(screeningVM.SelectedMovieId);

				var screenings = await _unitOfWork.Screening.FindAsync(x => x.StartTime.Date == screeningVM.Screening.StartTime.Date
									&& x.RoomId == screeningVM.Screening.RoomId, includeProperties: "Movie");

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

				await _unitOfWork.Screening.AddAsync(newScreening);
				await _unitOfWork.SaveAsync();
				return RedirectToAction("Details", "Room", new { id = screeningVM.Screening.RoomId });
			}
			return View();
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var screening = await _unitOfWork.Screening.GetByAsync(x => x.Id == id, includeProperties: "Movie,Room,Tickets");
			
			if (screening == null)
			{
				return NotFound();
			}

			return View(screening);
		}

		public async Task<IActionResult> Delete(int? id)
		{	
			if (id == null || id == 0)
			{
				return NotFound();
			}

			var screening = await _unitOfWork.Screening.GetByAsync(x => x.Id == id, includeProperties: "Tickets");

			if (screening == null)
			{
				return NotFound();
			}

			if (screening.Tickets.Count > 0)
			{
				return RedirectToAction("Details", "Screening", new { id });
			}

			_unitOfWork.Screening.Remove(screening);
			await _unitOfWork.SaveAsync();

			return RedirectToAction("Details", "Room", new {id = screening.RoomId});
		}
	}
}
