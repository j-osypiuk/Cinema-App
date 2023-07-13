using CinemaApp.Models.DomainModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaApp.Models.ViewModels
{
	public class MovieVM
	{
		public Movie Movie { get; set; }
		public ICollection<int> SelectedGenreIds { get; set; } = new List<int>();

		[ValidateNever]
		public IEnumerable<Genre> Genres { get; set; }
	}
}
