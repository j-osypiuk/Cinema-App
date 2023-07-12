namespace CinemaApp.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IGenreRepository Genre { get; }
        IMovieRepository Movie { get; }
        IMovieGenreRepository MovieGenre { get; }
        IRoomRepository Room { get; }
        IScreeningRepository Screening { get; }
        ITicketRepository Ticket { get; }
        IHomeContentRepository HomeContent { get; }
        Task SaveAsync();
    }
}
