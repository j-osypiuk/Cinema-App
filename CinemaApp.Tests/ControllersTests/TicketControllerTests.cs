using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using CinemaApp.Models.ViewModels;
using CinemaApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Moq;
using System.Linq.Expressions;

namespace CinemaApp.Tests.ControllersTests
{
	public class TicketControllerTests
	{
		private readonly TicketController _sut;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();

        public TicketControllerTests()
        {
            _sut = new TicketController(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task TicketController_Index_ReturnsANotFoundResultIfGivenMovieIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _sut.Index(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

		[Fact]
		public async Task TicketController_Index_ReturnsANotFoundResultIfGivenMovieIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Index(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task TicketController_Index_ReturnsANotFoundResultIfSelectedMovieHasNoAnyFutureScreenings()
		{
			// Arrange
			int? id = 1;
			List<Screening> screenings = null;
			_unitOfWorkMock.Setup(x => x.Screening.FindAsync(x => x.Movie.Id == id && x.StartTime >= DateTime.Now, "Movie,Room")).ReturnsAsync(screenings).Verifiable();

			// Act
			var result = await _sut.Index(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task TicketController_Index_ReturnsAViewResultWithScreeningListIfEverythingIsCorrect()
		{
			// Arrange
			int? id = 1;
			var screenings = new List<Screening>
			{
				new Screening { Id = 1 },
				new Screening { Id = 2 }
			};
			_unitOfWorkMock.Setup(x => x.Screening.FindAsync(x => x.Movie.Id == id && x.StartTime >= DateTime.Now, "Movie,Room")).ReturnsAsync(screenings).Verifiable();

			// Act
			var result = await _sut.Index(id);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<List<Screening>>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Count);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task TicketController_Create_ReturnsANotFoundResultIfGivenScreeningIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.Create(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task TicketController_Create_ReturnsANotFoundResultIfGivenScreeningIdIsZero()
		{
			// Arrange
			int? id = 0;

			// Act
			var result = await _sut.Create(id);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task TicketController_Create_ReturnsAViewResultWithTicketVMIfEverythingIsCorrect()
		{
			// Arrange
			int? id = 1;
			var movie = new Movie { Title = "Title", TicketPrice = 15, ImageUrl = "/null"  };
			var ticketsBought = new List<Ticket> { new Ticket { Id = 1 }, new Ticket { Id = 2 } };
			var screening = new Screening { Id = 1, RoomId = 1, Movie = movie, Tickets = ticketsBought };
			_unitOfWorkMock.Setup(x => x.Screening.GetByAsync(x => x.Id == id, "Movie,Tickets")).ReturnsAsync(screening).Verifiable();

			// Act
			var result = await _sut.Create(id);	

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);	
			var model = Assert.IsAssignableFrom<TicketVM>(viewResult.ViewData.Model);
			Assert.Equal(screening.Movie.TicketPrice, model.Ticket.Price);
			Assert.Equal(screening.RoomId, model.Ticket.RoomId);
			Assert.Equal(screening.Id, model.Ticket.ScreeningId);
			Assert.Equal(screening.Movie.Title, model.MovieTitle);
			Assert.Equal(ticketsBought.Count, model.TicketsBought.Count);
			Assert.Equal(screening.Movie.ImageUrl, model.MovieImageUrl);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task TicketController_CreateHttp_ReturnsARedirectToIndexActionOfHomeControllerIfModelStateIsInvalid()
		{
			// Arrange
			List<Ticket> tickets = null;
			TicketVM ticketVM = null;
			_sut.ModelState.AddModelError("", "");
			_unitOfWorkMock.Setup(x => x.Ticket.GetAllAsync(null)).ReturnsAsync(tickets).Verifiable();

			// Act
			var result = await _sut.Create(ticketVM);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.Equal("Home", redirectToActionResult.ControllerName);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task TicketController_CreateHttp_ReturnsAViewResultWithTicketVMIfWantedSeatIsAlreadyReserved()
		{
			// Arrange
			var ticketsBought = new List<Ticket> { new Ticket { Id = 1 }, new Ticket { Id = 2 } };
			var tickets = new List<Ticket> { new Ticket { Id = 1, Row = 1, Number = 1, ScreeningId = 1 } };
			var ticketVM = new TicketVM
			{
				SelectedRowNumber = 1,
				SelectedSeatNumber = 1,
				Ticket = new Ticket { ScreeningId = 1 }
			};

			_unitOfWorkMock.Setup(x => x.Ticket.GetAllAsync(null)).ReturnsAsync(tickets).Verifiable();
			_unitOfWorkMock.Setup(x => x.Ticket.FindAsync(x => x.ScreeningId == ticketVM.Ticket.ScreeningId, null)).ReturnsAsync(ticketsBought).Verifiable();

			// Act
			var result = await _sut.Create(ticketVM);

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<TicketVM>(viewResult.ViewData.Model);
			Assert.Equal(ticketsBought.Count, model.TicketsBought.Count);
			_unitOfWorkMock.Verify();
		}

		[Fact]
		public async Task TicketController_CreateHttp_ReturnsAViewResultWithTicketVMIfEverythingIsCorrect()
		{
			// Arrange
			var ticketsBought = new List<Ticket> { new Ticket { Id = 1 }, new Ticket { Id = 2 } };
			var tickets = new List<Ticket> { new Ticket { Id = 5, Row = 5, Number = 1, ScreeningId = 1 } };
			var ticketVM = new TicketVM
			{
				SelectedRowNumber = 1,
				SelectedSeatNumber = 1,
				Ticket = new Ticket { ScreeningId = 1 }
			};

			_unitOfWorkMock.Setup(x => x.Ticket.GetAllAsync(null)).ReturnsAsync(tickets).Verifiable();
			_unitOfWorkMock.Setup(x => x.Ticket.AddAsync(ticketVM.Ticket)).Returns(Task.CompletedTask).Verifiable();
			_unitOfWorkMock.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask).Verifiable();

			// Act
			var result = await _sut.Create(ticketVM);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.Equal("Home", redirectToActionResult.ControllerName);
			_unitOfWorkMock.Verify();
		}
	}
}
