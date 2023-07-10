using CinemaApp.Models.DomainModels;

namespace CinemaApp.Models.ViewModels
{
    public class HomeVM
    {
        public ICollection<Movie> Movies { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<string> PaginationImgUrls { get; set; }
        public ICollection<string> TextParagraphs { get; set; }
    }
}
