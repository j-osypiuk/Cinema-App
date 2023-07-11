using System.ComponentModel;

namespace CinemaApp.Models.DomainModels
{
    public class HomeContent
    {
        public int Id { get; set; }

        [DisplayName("First Carousel Image")]
        public string? CarouselImgUrl_1 { get; set; }

        [DisplayName("Second Carousel Image")]
        public string? CarouselImgUrl_2 { get; set; }

        [DisplayName("Third Carousel Image")]
        public string? CarouselImgUrl_3 { get; set; }

        [DisplayName("First Blog Title")]
        public string? BlogTitle_1 { get; set; }
        
        [DisplayName("Second Blog Title")]
        public string? BlogTitle_2 { get; set; }

        [DisplayName("Third Blog Title")]
        public string? BlogTitle_3 { get; set; }

        [DisplayName("First Blog Content")]
        public string? BlogContent_1 { get; set; }

        [DisplayName("Second Blog Content")]
        public string? BlogContent_2 { get; set; }

        [DisplayName("Third Blog Content")]
        public string? BlogContent_3 { get; set; }
    }
}
