using CinemaApp.Models.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace CinemaApp.Models.ViewModels
{
	public class ScreeningVM
	{
		public Screening Screening { get; set; }

		[DisplayName("Movie")]
		public int SelectedMovieId { get; set; }
		public IEnumerable<SelectListItem>? MovieSelectList { get; set; }
	}
}
