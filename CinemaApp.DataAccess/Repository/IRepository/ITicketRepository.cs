using CinemaApp.Models.DomainModels;

namespace CinemaApp.DataAccess.Repository.IRepository
{
	public interface ITicketRepository : IRepository<Ticket> 
	{
		void Update(Ticket ticket);
	}
}
