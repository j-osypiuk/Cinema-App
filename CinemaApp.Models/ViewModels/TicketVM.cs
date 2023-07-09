using CinemaApp.Models.DomainModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CinemaApp.Models.ViewModels
{
	public class TicketVM
	{
        public Ticket Ticket { get; set; }
        public string? MovieTitle { get; set; }
        public string? MovieImageUrl { get; set; }
        public ICollection<Ticket>? TicketsBought { get; set; }

        [DisplayName("Row Number")]
        public int SelectedRowNumber { get; set; }

        [DisplayName("Seat Number")]
        public int SelectedSeatNumber { get; set; }

        [DisplayName("Email Address")]
        public string UserEmail { get; set; }
    }
}
