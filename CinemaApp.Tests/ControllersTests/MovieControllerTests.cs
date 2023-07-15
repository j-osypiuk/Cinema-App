using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using CinemaApp.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CinemaApp.Tests.ControllersTests
{
	public class MovieControllerTests
	{
		private readonly MovieController _sut;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
		private readonly Mock<IWebHostEnvironment> __webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

		public MovieControllerTests()
		{
			_sut = new MovieController(_unitOfWorkMock.Object, __webHostEnvironmentMock.Object);
		}

		[Fact]
		public async Task MovieController_Index_ReturnsAViewResultWithAListOfMovies()
		{
			// Arrange 
			var movie_1 = new Movie { Id = 1 };
			var movie_2 = new Movie { Id = 2 };
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre { MovieId = movie_1.Id, Movie = movie_1, GenreId = 1 },
				new MovieGenre { MovieId = movie_2.Id, Movie = movie_2, GenreId = 2 },
				new MovieGenre { MovieId = movie_1.Id, Movie = movie_1, GenreId = 2 },
			};
			_unitOfWorkMock.Setup(x => x.MovieGenre.GetAllAsync("Movie,Genre")).ReturnsAsync(movieGenres).Verifiable();


			// Act
			var result = await _sut.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<List<Movie>>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Count());
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_Create_ReturnsAViewResultWithMovieVM()
		{
			// Arrange
			var genres = new List<Genre>
			{
				new Genre { Name = "Action" },
				new Genre { Name = "Drama"}
			};
			_unitOfWorkMock.Setup(x => x.Genre.GetAllAsync(null)).ReturnsAsync(genres).Verifiable();

			// Act
			var result = await _sut.Create();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<MovieVM>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Genres.Count());
			Assert.Equal(0, model.Movie.Id);
			Assert.Equal(DateTime.Today.Date, model.Movie.ReleaseDate.Date);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_CreateHttpPost_ReturnsAViewResultWithMovieVMIfModelStateIsNotValid()
		{
			// Arrange
			var movieVM = new MovieVM
			{
				Movie = new Movie { Id = 1 },
				SelectedGenreIds = { 1, 2 }
			};
			var genres = new List<Genre>
			{
				new Genre { Name = "Action" },
				new Genre { Name = "Drama"}
			};
			IFormFile formFile = null;
			_unitOfWorkMock.Setup(x => x.Genre.GetAllAsync(null)).ReturnsAsync(genres).Verifiable();
			_sut.ModelState.AddModelError("", "");

			// Act
			var result = await _sut.Create(movieVM, formFile);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<MovieVM>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Genres.Count());
			Assert.Equal(1, model.Movie.Id);
			Assert.Equal(2, model.SelectedGenreIds.Count());
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_CreateHttpPost_ReturnsAViewResultWithMovieVMIfNoGenreIdWereSelected()
		{
			// Arrange
			var movieVM = new MovieVM { Movie = new Movie { Id = 1 } };
			var genres = new List<Genre>
			{
				new Genre { Name = "Action" },
				new Genre { Name = "Drama"}
			};
			IFormFile formFile = null;
			_unitOfWorkMock.Setup(x => x.Genre.GetAllAsync(null)).ReturnsAsync(genres).Verifiable();

			// Act
			var result = await _sut.Create(movieVM, formFile);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<MovieVM>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Genres.Count());
			Assert.Equal(movieVM.Movie.Id, model.Movie.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_CreateHttpPost_RedirectsToIndexActionIfModelStateIsValid()
		{
			// Arrange
			var movieVM = new MovieVM { Movie = new Movie { Id = 1 }, SelectedGenreIds = { 1, 2 } };
			IFormFile formFile = null;
			_unitOfWorkMock.Setup(x => x.Movie.AddAsync(movieVM.Movie)).Returns(Task.CompletedTask).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

			// Act
			var result = await _sut.Create(movieVM, formFile);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_Details_ReturnsAViewResultWithMovie()
		{
			// Arrange
			int? id = 1;
			var movie = new Movie { Id = 1 };
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre { MovieId = movie.Id, Movie = movie, GenreId = 1},
				new MovieGenre { MovieId = movie.Id, Movie = movie, GenreId = 2},
			};
			movie.MovieGenres = movieGenres;
			_unitOfWorkMock.Setup(x => x.MovieGenre.FindAsync(x => x.MovieId == id, "Movie,Genre")).ReturnsAsync(movieGenres).Verifiable();

			// Act
			var result = await _sut.Details(id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<Movie>(viewResult.ViewData.Model);
			Assert.Equal(id, model.Id);
			Assert.Equal(2, model.MovieGenres.Count);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_Edit_ReturnsAViewResultWithAMovieVMIfCorrespondingMovieIsFound()
		{
			// Arragne
			int? id = 1;
			var movie = new Movie { Id = 1 };
			var genres = new List<Genre>
			{
				new Genre { Id = 1 },
				new Genre { Id = 2 },
			};
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre { MovieId = movie.Id, Movie = movie, GenreId = 1},
				new MovieGenre { MovieId = movie.Id, Movie = movie, GenreId = 2},
			};
			movie.MovieGenres = movieGenres;
			_unitOfWorkMock.Setup(x => x.MovieGenre.FindAsync(x => x.MovieId == id, "Movie,Genre")).ReturnsAsync(movieGenres).Verifiable();
			_unitOfWorkMock.Setup(x => x.Genre.GetAllAsync(null)).ReturnsAsync(genres).Verifiable();

			// Act
			var result = await _sut.Edit(id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<MovieVM>(viewResult.ViewData.Model);
			Assert.Equal(id, model.Movie.Id);
			Assert.Equal(2, model.SelectedGenreIds.Count);
			Assert.Equal(2, model.Movie.MovieGenres.Count);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_Edit_ReturnsANotFoundIfGivenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Edit(id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
		}


		[Fact]
		public async Task MovieController_Edit_ReturnsANotFoundIfGivenIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Edit(id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task MovieController_Edit_ReturnsANotFoundIfWantedMovieIsNotFound()
		{	
			// Arrange
			int? id = 1;
			Movie movie = null;
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre { MovieId = 1, Movie = movie, GenreId = 1},
				new MovieGenre { MovieId = 1, Movie = movie, GenreId = 2},
			};
			_unitOfWorkMock.Setup(x => x.MovieGenre.FindAsync(x => x.MovieId == id, "Movie,Genre")).ReturnsAsync(movieGenres).Verifiable();

			// Act
			var result = await _sut.Edit(id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_EditHttpPost_ReturnsAViewResultWithMovieVMIfModelStateIsNotValid()
		{
			// Arrange
			var movieVM = new MovieVM
			{
				Movie = new Movie { Id = 1 },
				SelectedGenreIds = { 1, 2 }
			};
			var genres = new List<Genre>
			{
				new Genre { Name = "Action" },
				new Genre { Name = "Drama"}
			};
			IFormFile formFile = null;
			_unitOfWorkMock.Setup(x => x.Genre.GetAllAsync(null)).ReturnsAsync(genres).Verifiable();
			_sut.ModelState.AddModelError("", "");

			// Act
			var result = await _sut.Edit(movieVM, formFile);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<MovieVM>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Genres.Count());
			Assert.Equal(1, model.Movie.Id);
			Assert.Equal(2, model.SelectedGenreIds.Count());
			_unitOfWorkMock.Verify();
		}


		[Fact]
		public async Task MovieController_EditHttpPost_ReturnsAViewResultWithMovieVMIfNoGenreIdWereSelected()
		{
			// Arrange
			var movieVM = new MovieVM { Movie = new Movie { Id = 1 } };
			var genres = new List<Genre>
			{
				new Genre { Name = "Action" },
				new Genre { Name = "Drama"}
			};
			IFormFile formFile = null;
			_unitOfWorkMock.Setup(x => x.Genre.GetAllAsync(null)).ReturnsAsync(genres).Verifiable();

			// Act
			var result = await _sut.Edit(movieVM, formFile);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<MovieVM>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Genres.Count());
			Assert.Equal(movieVM.Movie.Id, model.Movie.Id);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_EditHttpPost_RedirectsToIndexActionIfModelStateIsValid()
		{
			// Arrange
			var movie = new Movie { Id = 1 };
			var movieVM = new MovieVM { Movie = movie, SelectedGenreIds = { 1, 3 } };
			var movieGenres = new List<MovieGenre>
			{
				new MovieGenre { MovieId = movie.Id, Movie = movie, GenreId = 1},
				new MovieGenre { MovieId = movie.Id, Movie = movie, GenreId = 2},
			};
			movie.MovieGenres = movieGenres;
			IFormFile formFile = null;
			_unitOfWorkMock.Setup(x => x.MovieGenre.FindAsync(x => x.MovieId == movieVM.Movie.Id, null)).ReturnsAsync(movieGenres).Verifiable();
			_unitOfWorkMock.Setup(x => x.MovieGenre.Remove(movieGenres[1])).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.Update(movieVM.Movie)).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

			// Act
			var result = await _sut.Edit(movieVM, formFile);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Null(redirectToActionResult.ControllerName);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task MovieController_Delete_ReturnsANotFoundIfGivenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Delete(id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task MovieController_Delete_ReturnsANotFoundIfGivenIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Delete(id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task MovieController_Delete_ReturnsANotFoundIfWantedMovieIsNotFound()
		{	
			// Arrange
			int? id = 1;
			Movie movie = null;
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(id)).ReturnsAsync(movie).Verifiable();

			// Act
			var result = await _sut.Delete(id);

			// Assert
			var viewResult = Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task MovieController_Delete_ReturnsAViewIfGivenMovieIsDeleted()
		{
			// Arrange
			int? id = 1;
			Movie movie = new Movie { Id = 1 };
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(id)).ReturnsAsync(movie).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.Remove(movie)).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

			// Act
			var result = await _sut.Delete(id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			_unitOfWorkMock.Verify();
		}

	}
}
