using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace CinemaApp.Tests.ControllersTests
{
	public class GenreControllerTests
	{
		private readonly GenreController _sut;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        public GenreControllerTests()
        {
			_sut = new GenreController(_unitOfWorkMock.Object);
        }

        [Fact]
		public async Task GenreController_Index_ReturnsAViewResultWithAListOfGenres()
		{
			// Arrange 
			var genres = new List<Genre>
			{
				new Genre { Name = "Action" },
				new Genre { Name = "Drama"}
			};
			_unitOfWorkMock.Setup(x => x.Genre.GetAllAsync(null)).ReturnsAsync(genres).Verifiable();

			// Act
			var result = await _sut.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<Genre>>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Count());
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task GenreController_GenreMovies_ReturnsANotFoundIfGivenIdIsNull()
		{	
			// Arrange
			int? genreId = null;

			// Act
			var result = await _sut.GenreMovies(genreId);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task GenreController_GenreMovies_ReturnsANotFoundIfGivenIdIsZero()
		{
			// Arrange
			int? genreId = 0;

			// Act
			var result = await _sut.GenreMovies(genreId);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task GenreController_GenreMovies_ReturnsViewResultWithGenreMoviesListIfWantedGenreIsExists()
		{
			// Arrange
			var tempDataMock = new Mock<ITempDataDictionary>();
			var genreController = new GenreController(_unitOfWorkMock.Object) { TempData = tempDataMock.Object };
			var movie_1 = new Movie { Id = 1 };
			var movie_2 = new Movie { Id = 2 };
			var genre_1 = new Genre { Id = 1, Name = "Action" };
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre { MovieId = movie_1.Id, Movie = movie_1, GenreId = genre_1.Id, Genre = genre_1 },
				new MovieGenre { MovieId = movie_2.Id, Movie = movie_2, GenreId = genre_1.Id, Genre = genre_1 },
			};
			_unitOfWorkMock.Setup(x => x.MovieGenre.GetAllAsync("Movie,Genre")).ReturnsAsync(movieGenres).Verifiable();

			// Act
			var result = await genreController.GenreMovies(genre_1.Id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<List<Movie>>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Count);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task GenreController_CreateHttpPost_RedirectsToIndexActionIfModelStateIsValid()
		{
			// Arrange
			var genre = new Genre { Name  = "Action" };
			_unitOfWorkMock.Setup(x => x.Genre.AddAsync(genre)).Returns(Task.CompletedTask).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

			// Act
			var result = await _sut.Create(genre);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task GenreController_CreateHttpPost_ReturnsViewResultIfModelStateIsNotValid()
		{
			// Arrange
			var genre = new Genre { Name = "Action" };
			_sut.ModelState.AddModelError("", "");

			// Act
			var result = await _sut.Create(genre);

			// Assert
			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task GenreController_Edit_ReturnsAViewResultWithAGenreIfCorrespondingGenreIsFound()
		{
			// Arrange
			int? id = 1;
			var genre = new Genre { Id = 1, Name = "Action" };
			_unitOfWorkMock.Setup(x => x.Genre.GetAsync(id)).ReturnsAsync(genre).Verifiable();

			// Act
			var result = await _sut.Edit(id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<Genre>(viewResult.ViewData.Model);
			Assert.Equal(genre.Id, model.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task GenreController_Edit_ReturnsANotFoundIfGivenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Edit(id: id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task GenreController_Edit_ReturnsANotFoundIfGivenIdIsEqualZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Edit(id: id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}


		[Fact]
		public async Task GenreController_Edit_ReturnsANotFoundIfWantedGenreIsNotFound()
		{
			// Arrange
			int? id = 1;
			Genre genre = null;
			_unitOfWorkMock.Setup(x => x.Genre.GetAsync(id)).ReturnsAsync(genre).Verifiable();

			// Act
			var result = await _sut.Edit(id: id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task GenreController_EditHttpPost_RedirectsToIndexActionIfModelStateIsValid()
		{
			// Arrange
			var genre = new Genre { Name = "Action" };
			_unitOfWorkMock.Setup(x => x.Genre.Update(genre)).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

			// Act
			var result = await _sut.Edit(genre);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task GenreController_EditHttpPost_ReturnsViewWithGenreIfModelStateIsNotValid()
		{
			// Arrange
			var genre = new Genre { Id = 1 };
			_sut.ModelState.AddModelError("", "");

			// Act
			var result = await _sut.Edit(genre);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<Genre>(viewResult.ViewData.Model);
			Assert.Equal(1, model.Id);
		}

		[Fact]
		public async Task GenreController_Delete_ReturnsANotFoundIfGivenIdIsNull()
		{
			// Arrange
			int? id = null;
			var genre = new Genre { Id = 1};

			// Act
			var result = await _sut.Delete(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task GenreController_Delete_ReturnsANotFoundIfGivenIdIsEqualZero()
		{
			// Arrange
			int? id = 0;
			var genre = new Genre { Id = 1 };

			// Act
			var result = await _sut.Delete(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}


		[Fact]
		public async Task GenreController_Delete_ReturnsANotFoundIfWantedGenreIsNotFound()
		{
			// Arrange
			int? id = 1;
			Genre genre = null;
			_unitOfWorkMock.Setup(x => x.Genre.GetAsync(id)).ReturnsAsync(genre).Verifiable();

			// Act
			var result = await _sut.Delete(id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task GenreController_Delete_RedirectsToIndexActionIfGivenGenreIsDeleted()
		{
			// Arrange
			int? id = 1;
			var genre = new Genre { Id = 1 };
			_unitOfWorkMock.Setup(x => x.Genre.GetAsync(id)).ReturnsAsync(genre).Verifiable();
			_unitOfWorkMock.Setup(x => x.Genre.Remove(genre)).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Verifiable();
			
			// Act
			var result = await _sut.Delete(id);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			_unitOfWorkMock.Verify();
		}
	}
}
