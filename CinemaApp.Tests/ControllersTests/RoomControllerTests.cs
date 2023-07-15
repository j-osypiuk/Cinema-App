using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CinemaApp.Tests.ControllersTests
{
	public class RoomControllerTests
	{
		private readonly RoomController _sut;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        public RoomControllerTests()
        {
            _sut = new RoomController(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task RoomController_Index_ReturnsANotFoundResultIfNoRoomExists()
        {   
            // Arrange
            List<Room> rooms = null;
            _unitOfWorkMock.Setup(x => x.Room.GetAllAsync(null)).ReturnsAsync(rooms).Verifiable();

            // Act
            var result = await _sut.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
        }


		[Fact]
		public async Task RoomController_Index_ReturnsAViewResultWithListOfRooms()
		{
			// Arrange
			var rooms = new List<Room>
			{	
				new Room { Id = 1 },
				new Room { Id = 2 }
			};
			_unitOfWorkMock.Setup(x => x.Room.GetAllAsync(null)).ReturnsAsync(rooms).Verifiable();

			// Act
			var result = await _sut.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<List<Room>>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Count);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task RoomController_Details_ReturnsANotFoundResultIfIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Details(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task RoomController_Details_ReturnsANotFoundResultIfIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Details(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task RoomController_Details_ReturnsANotFoundResultIfWantedRoomDoesNotExists()
		{
			// Arrange
			int? id = 1;
			Room room = null;
			_unitOfWorkMock.Setup(x => x.Room.GetByAsync(x => x.Id == id, "Screenings")).ReturnsAsync(room).Verifiable();

			// Act
			var result = await _sut.Details(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task RoomController_Details_ReturnsAViewResultWithRoom()
		{
			// Arrange
			int? id = 1;
			var movie = new Movie { Id = 1 };
			var screening = new Screening { Id = 1, MovieId = movie.Id, Movie = movie, StartTime = DateTime.Today.AddDays(1)};
			var screenings = new List<Screening> { screening };
			var room = new Room { Id = 1, Screenings = screenings };
			_unitOfWorkMock.Setup(x => x.Room.GetByAsync(x => x.Id == id, "Screenings")).ReturnsAsync(room).Verifiable();
			_unitOfWorkMock.Setup(x => x.Movie.GetAsync(screening.Movie.Id)).ReturnsAsync(screening.Movie).Verifiable();

			// Act
			var result = await _sut.Details(id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<Room>(viewResult.ViewData.Model);
			Assert.Equal(room.Id, model.Id);
			Assert.Equal(1, model.Screenings.Count);
			_unitOfWorkMock.Verify();
		}
	}
}
