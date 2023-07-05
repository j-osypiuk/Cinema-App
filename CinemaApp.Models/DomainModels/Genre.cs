using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaApp.Models.DomainModels
{
	public class Genre
	{
        public int Id { get; set; }
		public string Name { get; set; }

		[ValidateNever]
        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
