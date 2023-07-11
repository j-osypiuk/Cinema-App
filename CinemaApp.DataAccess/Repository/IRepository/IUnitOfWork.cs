namespace CinemaApp.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IGenreRepository Genre { get; }
        Task SaveAsync();
    }
}
