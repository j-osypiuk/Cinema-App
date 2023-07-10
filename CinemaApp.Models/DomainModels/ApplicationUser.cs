using Microsoft.AspNetCore.Identity;

namespace CinemaApp.Models.DomainModels
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime StartJob { get; set; }
    }
}
