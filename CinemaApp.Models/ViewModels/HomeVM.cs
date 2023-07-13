using CinemaApp.Models.DomainModels;

namespace CinemaApp.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public HomeContent HomeContent { get; set; }
    }
}
