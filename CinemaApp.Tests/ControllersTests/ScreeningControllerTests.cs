using CinemaApp.DataAccess.Repository;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using CinemaApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System.Numerics;

namespace CinemaApp.Tests.ControllersTests
{
	public class ScreeningControllerTests
	{
		private readonly ScreeningController _sut;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        public ScreeningControllerTests()
		{
			_sut = new ScreeningController(_unitOfWorkMock.Object);
		}

        [Fact]
        public async Task ScreeningController_Index_ReturnsANotFoundResultIfNoScreeningExists()
        {
            // Arrange
            List<Screening> screenings = null;
            _unitOfWorkMock.Setup(x => x.Screening.GetAllAsync("Movie,Room")).ReturnsAsync(screenings).Verifiable();

            // Act 
            var result = await _sut.Index();

            // Arrange
            Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
        }


		[Fact]
		public async Task ScreeningController_Index_ReturnsAViewResultWithScreeningList()
		{
			// Arrange
			var screenings = new List<Screening>
			{
				new Screening { Id = 1 },
				new Screening { Id = 2 }
			};
			_unitOfWorkMock.Setup(x => x.Screening.GetAllAsync("Movie,Room")).ReturnsAsync(screenings).Verifiable();

			// Act 
			var result = await _sut.Index();

			// Arrange
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<List<Screening>>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Count);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_Create_ReturnsANotFoundResultIfGivenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Create(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Create_ReturnsANotFoundResultIfGivenIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Create(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Create_ReturnsANotFoundIfNoMovieExists()
		{
			// Arrange
			int? id = 1;
			List<Movie> movies = null;
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();

			// Act
			var result = await _sut.Create(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
		}


		[Fact]
		public async Task ScreeningController_CreateHttp_ReturnsAViewResultWithScreeningVMIfModelStateIsNotValid()
		{
			// Arrange
			var screeningVM = new ScreeningVM { Screening = new Screening { Id = 1 } };
			var movies = new List<Movie> 
			{ 
				new Movie { Id = 1, Title = "MovieTitle_1" },
				new Movie { Id = 2, Title = "MovieTitle_2" }
			};
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();
			_sut.ModelState.AddModelError("", "");

			// Act
			var result = await _sut.Create(screeningVM);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<ScreeningVM>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Screening.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_CreateHttp_ReturnsAViewResultWithScreeningVMIfSelectedMovieDoesNotExists()
		{
			// Arrange
			Movie movie = null;
			var movies = new List<Movie>
			{
				new Movie { Id = 1, Title = "MovieTitle_1" },
				new Movie { Id = 2, Title = "MovieTitle_2" }
			};
			var screeningVM = new ScreeningVM 
			{
				Screening = new Screening { Id = 1 },
				SelectedMovieId = 1
			};
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(screeningVM.SelectedMovieId)).ReturnsAsync(movie).Verifiable();	
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();

			// Act
			var result = await _sut.Create(screeningVM);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<ScreeningVM>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Screening.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_CreateHttp_ReturnsAViewResultWithScreeningVMIfThePlannedTimeOfTheScreeningIsIncludedInTheTimeOfAnotherScreening()
		{
			// Arrange
			var movie_1 = new Movie { Id = 1, Title = "MovieTitle_1", Duration = 30 };
			var movie_2 = new Movie { Id = 2, Title = "MovieTitle_2", Duration = 100 };
			var movies = new List<Movie> { movie_2 };
			var screeningVM = new ScreeningVM
			{
				Screening = new Screening { Id = 1, StartTime = DateTime.Today.AddDays(2).AddMinutes(30), Movie = movie_1, RoomId = 1 },
				SelectedMovieId = 1
			};
			var screenings = new List<Screening>
			{
				new Screening { Id = 1, StartTime = DateTime.Today.AddDays(2), Movie = movie_2, RoomId = 1 }
			};
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(screeningVM.SelectedMovieId)).ReturnsAsync(movie_1).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();
			_unitOfWorkMock.Setup(x => x.Screening.FindAsync(x => x.StartTime.Date == screeningVM.Screening.StartTime.Date
									&& x.RoomId == screeningVM.Screening.RoomId, "Movie")).ReturnsAsync(screenings).Verifiable();
			
			// Act
			var result = await _sut.Create(screeningVM);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<ScreeningVM>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Screening.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_CreateHttp_ReturnsAViewResultWithScreeningVMIfThePlannedStartTimeOfTheScreeningIsIncludedInTheTimeOfAnotherScreening()
		{
			// Arrange
			var movie_1 = new Movie { Id = 1, Title = "MovieTitle_1", Duration = 150 };
			var movie_2 = new Movie { Id = 2, Title = "MovieTitle_2", Duration = 100 };
			var movies = new List<Movie> { movie_2 };
			var screeningVM = new ScreeningVM
			{
				Screening = new Screening { Id = 1, StartTime = DateTime.Today.AddDays(2).AddMinutes(30), Movie = movie_1, RoomId = 1 },
				SelectedMovieId = 1
			};
			var screenings = new List<Screening>
			{
				new Screening { Id = 1, StartTime = DateTime.Today.AddDays(2), Movie = movie_2, RoomId = 1 }
			};
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(screeningVM.SelectedMovieId)).ReturnsAsync(movie_1).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();
			_unitOfWorkMock.Setup(x => x.Screening.FindAsync(x => x.StartTime.Date == screeningVM.Screening.StartTime.Date
									&& x.RoomId == screeningVM.Screening.RoomId, "Movie")).ReturnsAsync(screenings).Verifiable();

			// Act
			var result = await _sut.Create(screeningVM);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<ScreeningVM>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Screening.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_CreateHttp_ReturnsAViewResultWithScreeningVMIfThePlannedEndTimeOfTheScreeningIsIncludedInTheTimeOfAnotherScreening()
		{
			// Arrange
			var movie_1 = new Movie { Id = 1, Title = "MovieTitle_1", Duration = 50 };
			var movie_2 = new Movie { Id = 2, Title = "MovieTitle_2", Duration = 100 };
			var movies = new List<Movie> { movie_2 };
			var screeningVM = new ScreeningVM
			{
				Screening = new Screening { Id = 1, StartTime = DateTime.Today.AddDays(2), Movie = movie_1, RoomId = 1 },
				SelectedMovieId = 1
			};
			var screenings = new List<Screening>
			{
				new Screening { Id = 1, StartTime = DateTime.Today.AddDays(2).AddMinutes(25), Movie = movie_2, RoomId = 1 }
			};
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(screeningVM.SelectedMovieId)).ReturnsAsync(movie_1).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();
			_unitOfWorkMock.Setup(x => x.Screening.FindAsync(x => x.StartTime.Date == screeningVM.Screening.StartTime.Date
									&& x.RoomId == screeningVM.Screening.RoomId, "Movie")).ReturnsAsync(screenings).Verifiable();

			// Act
			var result = await _sut.Create(screeningVM);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<ScreeningVM>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Screening.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_CreateHttp_ReturnsAViewResultWithScreeningVMIfStartTimeOfScreeningIsNotAtLeastTomorrow()
		{
			// Arrange
			var movie_1 = new Movie { Id = 1, Title = "MovieTitle_1", Duration = 50 };
			var movie_2 = new Movie { Id = 2, Title = "MovieTitle_2", Duration = 100 };
			var movies = new List<Movie> { movie_2 };
			var screeningVM = new ScreeningVM
			{
				Screening = new Screening { Id = 1, StartTime = DateTime.Today, Movie = movie_1, RoomId = 1 },
				SelectedMovieId = 1
			};
			List<Screening> screenings = null;
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(screeningVM.SelectedMovieId)).ReturnsAsync(movie_1).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();
			_unitOfWorkMock.Setup(x => x.Screening.FindAsync(x => x.StartTime.Date == screeningVM.Screening.StartTime.Date
									&& x.RoomId == screeningVM.Screening.RoomId, "Movie")).ReturnsAsync(screenings).Verifiable();

			// Act
			var result = await _sut.Create(screeningVM);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<ScreeningVM>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Screening.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_CreateHttp_ReturnsRedirectToDetailsActionInRoomControllerWithRoomIdIfEverythingIsCorrect()
		{
			// Arrange
			var movie_1 = new Movie { Id = 1, Title = "MovieTitle_1", Duration = 50 };
			var movie_2 = new Movie { Id = 2, Title = "MovieTitle_2", Duration = 100 };
			var movies = new List<Movie> { movie_2 };
			var screeningVM = new ScreeningVM
			{
				Screening = new Screening { Id = 1, StartTime = DateTime.Today.AddDays(2), Movie = movie_1, RoomId = 1 },
				SelectedMovieId = 1
			};
			List<Screening> screenings = null;
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(screeningVM.SelectedMovieId)).ReturnsAsync(movie_1).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.GetAllAsync(null)).ReturnsAsync(movies).Verifiable();
			_unitOfWorkMock.Setup(x => x.Screening.FindAsync(x => x.StartTime.Date == screeningVM.Screening.StartTime.Date
									&& x.RoomId == screeningVM.Screening.RoomId, "Movie")).ReturnsAsync(screenings).Verifiable();
			_unitOfWorkMock.Setup(x => x.Screening.AddAsync(screeningVM.Screening));
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

			// Act
			var result = await _sut.Create(screeningVM);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Details", redirectToActionResult.ActionName);
			Assert.Equal("Room", redirectToActionResult.ControllerName);
			Assert.Equal(screeningVM.Screening.RoomId, redirectToActionResult.RouteValues["id"]);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_Details_ReturnsANotFoundResultIfGivenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Details(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Details_ReturnsANotFoundResultIfGivenIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Details(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Details_ReturnsANotFoundResultIfWantedScreeningDoesNotExists()
		{
			// Arrange
			int? id = 1;
			Screening screening = null;
			_unitOfWorkMock.Setup(x => x.Screening.GetByAsync(x => x.Id == id, "Movie,Room,Tickets")).ReturnsAsync(screening).Verifiable();

			// Act
			var result = await _sut.Details(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Details_ReturnsAViewResultWithScreeningIfWantedScreeningExists()
		{
			// Arrange
			int? id = 1;
			var screening = new Screening { Id = 1 };
			_unitOfWorkMock.Setup(x => x.Screening.GetByAsync(x => x.Id == id, "Movie,Room,Tickets")).ReturnsAsync(screening).Verifiable();

			// Act
			var result = await _sut.Details(id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<Screening>(viewResult.ViewData.Model);
			Assert.Equal(id, model.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task ScreeningController_Delete_ReturnsANotFoundResultIfGivenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Delete(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Delete_ReturnsANotFoundResultIfGivenIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Delete(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Delete_ReturnsANotFoundResultIfScreeningToDeleteDoesNotExists()
		{
			// Arrange
			int? id = 1;
			Screening screening = null;
			_unitOfWorkMock.Setup(x => x.Screening.GetByAsync(x => x.Id == id, "Movie,Room,Tickets")).ReturnsAsync(screening).Verifiable();

			// Act
			var result = await _sut.Delete(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task ScreeningController_Delete_ReturnsARedirectToDetailsActionWithIdIfScreeningToDeleteHasSoldTickets()
		{
			// Arrange
			int? id = 1;
			var tickets = new List<Ticket>
			{
				new Ticket { Id = 1 }
			};
			var screening = new Screening { Id = 1, Tickets = tickets };
			_unitOfWorkMock.Setup(x => x.Screening.GetByAsync(x => x.Id == id, "Tickets")).ReturnsAsync(screening).Verifiable();

			// Act
			var result = await _sut.Delete(id);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Details", redirectToActionResult.ActionName);
			Assert.Equal("Screening", redirectToActionResult.ControllerName);
			Assert.Equal(id, redirectToActionResult.RouteValues["id"]);
			_unitOfWorkMock.Verify();
		}


		[Fact]
		public async Task ScreeningController_Delete_ReturnsARedirectToDetailsActionOfRoomCotrollerWithRoomIdIfManageToDeleteScreening()
		{
			// Arrange
			int? id = 1;
			var tickets = new List<Ticket>();
			var screening = new Screening { Id = 1, Tickets = tickets, RoomId = 1 };
			_unitOfWorkMock.Setup(x => x.Screening.GetByAsync(x => x.Id == id, "Tickets")).ReturnsAsync(screening).Verifiable();
			_unitOfWorkMock.Setup(x => x.Screening.Remove(screening)).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

			// Act
			var result = await _sut.Delete(id);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Details", redirectToActionResult.ActionName);
			Assert.Equal("Room", redirectToActionResult.ControllerName);
			Assert.Equal(screening.RoomId, redirectToActionResult.RouteValues["id"]);
			_unitOfWorkMock.Verify();
		}
	}
}
