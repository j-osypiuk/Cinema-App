using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using CinemaApp.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CinemaApp.Tests.ControllersTests
{
	public class HomeControllerTests
	{
		private readonly HomeController _sut;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
		private readonly Mock<IWebHostEnvironment> _webHostEnvironment = new Mock<IWebHostEnvironment>();

		public HomeControllerTests()
		{
			_sut = new HomeController(_unitOfWorkMock.Object, _webHostEnvironment.Object);
		}

		[Fact]
		public async Task HomeController_Index_ReturnsANotFoundResultIfHomeContentDoesNotExists()
		{
			// Arrange
			List<HomeContent> homeContent = null;
			var movie_1 = new Movie { Id = 1 };
			var genre_1 = new Genre { Id = 1 };
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre { MovieId = movie_1.Id, Movie = movie_1, GenreId = genre_1.Id, Genre = genre_1 }
			};
			_unitOfWorkMock.Setup(x => x.MovieGenre.GetAllAsync("Movie,Genre")).ReturnsAsync(movieGenres).Verifiable();
			_unitOfWorkMock.Setup(x => x.HomeContent.GetAllAsync(null)).ReturnsAsync(homeContent).Verifiable();

			// Act
			var result = await _sut.Index();

			// Assert
			Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
		}

			[Fact]
		public async Task HomeController_Index_ReturnsAViewResultWithAHomeVM()
		{
			// Arrange
			var movie_1 = new Movie { Id = 1 };
			var movie_2 = new Movie { Id = 2 };
			var genre_1 = new Genre { Id = 1 };
			var genre_2 = new Genre { Id = 2 };
			var homeContent = new List<HomeContent> { new HomeContent { Id = 1 } };
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre {MovieId = movie_1.Id, Movie = movie_1, GenreId = genre_1.Id, Genre = genre_1 },
				new MovieGenre {MovieId = movie_2.Id, Movie = movie_2, GenreId = genre_2.Id, Genre = genre_2 },
				new MovieGenre {MovieId = movie_2.Id, Movie = movie_2, GenreId = genre_1.Id, Genre = genre_1 },
			};
			_unitOfWorkMock.Setup(x => x.MovieGenre.GetAllAsync("Movie,Genre")).ReturnsAsync(movieGenres).Verifiable();
			_unitOfWorkMock.Setup(x => x.HomeContent.GetAllAsync(null)).ReturnsAsync(homeContent).Verifiable();

			// Act
			var result = await _sut.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<HomeVM>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Movies.Count());
			Assert.Equal(2, model.Genres.Count());
			Assert.Equal(homeContent[0].Id, model.HomeContent.Id);
		}

		[Fact]
		public async Task HomeController_Edit_ReturnsANotFoundResultIfHomeContnentDoesNotExists()
		{
			// Arrange
			List<HomeContent> homeContent = null;
			_unitOfWorkMock.Setup(x => x.HomeContent.GetAllAsync(null)).ReturnsAsync(homeContent).Verifiable();

			// Act
			var result = await _sut.Edit();

			// Assert
			Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task HomeController_Edit_ReturnsAViewResultWithHomeContentIfHomeContnentExists()
		{
			// Arrange
			var homeContent = new List<HomeContent> { new HomeContent { Id = 1, } };
			_unitOfWorkMock.Setup(x => x.HomeContent.GetAllAsync(null)).ReturnsAsync(homeContent).Verifiable();

			// Act
			var result = await _sut.Edit();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<HomeContent>(viewResult.ViewData.Model);
			Assert.Equal(homeContent[0].Id, model.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task HomeController_EditHttp_ReturnsAViewResultWithHomeContentIfModelStateIsNotValid()
		{
			// Arrange
			var homeContent = new HomeContent { Id = 1 };
			_sut.ModelState.AddModelError("", "");

			// Act
			var result = await _sut.Edit(homeContent, null, null, null);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<HomeContent>(viewResult.ViewData.Model);
			Assert.Equal(homeContent.Id, model.Id);
		}

		[Fact]
		public async Task HomeController_EditHttp_ReturnsARedirectToIndexActionIfModelStateIsValid()
		{
			// Arrange
			var homeContent = new HomeContent { Id = 1 };
			_unitOfWorkMock.Setup(x => x.HomeContent.Update(homeContent)).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();
			
			// Act
			var result = await _sut.Edit(homeContent, null, null, null);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			_unitOfWorkMock.Verify();
		}
	}
}
