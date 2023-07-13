using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.DataAccess.Repository
{
	public class HomeContentRepository : Repository<HomeContent>, IHomeContentRepository
	{
        public HomeContentRepository(ApplicationDbContext db) : base(db) { }
		public void Update(HomeContent homeContent)
		{
			_dbSet.Update(homeContent);
		}
	}
}
