using CinemaApp.DataAccess.Data;
using CinemaApp.DataAccess.Repository.IRepository;
using CinemaApp.Models.DomainModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaApp.DataAccess.Repository
{
	public class ScreeningRepository : Repository<Screening>, IScreeningRepository
	{
        public ScreeningRepository(ApplicationDbContext db) : base(db) { }

		public void Update(Screening screening)
		{
			_dbSet.Update(screening);
		}
	}
}
