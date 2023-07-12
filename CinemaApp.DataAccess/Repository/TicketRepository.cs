using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;

namespace CinemaApp.DataAccess.Repository
{
	public class TicketRepository : Repository<Ticket>, ITicketRepository
	{
        public TicketRepository(ApplicationDbContext db) : base(db) { }

		public void Update(Ticket ticket)
		{
			_dbSet.Update(ticket);
		}
	}
}
