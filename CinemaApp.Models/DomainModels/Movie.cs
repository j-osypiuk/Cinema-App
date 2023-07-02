using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CinemaApp.Models.DomainModels
{
	public class Movie
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		[DisplayName("Release Date")]
		public DateTime ReleaseDate { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Movie must be longer than or equal to one minute.")]
		public double Duration { get; set; }

		[ValidateNever]
		public ICollection<MovieGenre> MovieGenres { get; set; }

		[DisplayName("Main Actor")]
		public string MainActor { get; set; }
		public string Director { get; set; }

		[Required]
		[DisplayName("Ticket Price")]
		[Range(1, int.MaxValue, ErrorMessage = "Ticket price must be more expensive than 1.")]
		public double TicketPrice { get; set; }
		public string? ImageUrl { get; set; }
	}
}
