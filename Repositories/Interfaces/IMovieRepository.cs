using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllAsync(
            int skip,
            int take,
            string? title);

        Task<Movie?> GetByIdAsync(int movieId);

        Task AddAsync(Movie movie);

        void Update(Movie movie);

        void Delete(Movie movie);

        Task<bool> ExistsAsync(int movieId);
    }
}
