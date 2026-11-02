using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<List<Show>> GetAllAsync(
            int skip, int take,
            DateOnly? date, int? movieId, int? theatreId);

        Task<int> GetAllCountAsync(
            DateOnly? date, int? movieId, int? theatreId);

        Task<Show?> GetByIdAsync(int showId);

        Task<List<Show>> GetByMovieIdAsync(
            int movieId, int skip, int take);

        Task<int> GetByMovieCountAsync(int movieId);

        Task<List<Show>> GetByTheatreIdAsync(
            int theatreId, int skip, int take);

        Task<int> GetByTheatreCountAsync(int theatreId);

        Task<List<Show>> GetByDateAsync(
            DateOnly date, int skip, int take);

        Task<int> GetByDateCountAsync(DateOnly date);

        Task<bool> MovieExistsAsync(int movieId);
        Task<bool> ScreenExistsAsync(int screenId);

        Task<List<ShowSeat>> GetShowSeatsAsync(int showId);
        Task<List<ShowSeat>> GetAvailableShowSeatsAsync(int showId);
        Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId);

        Task<bool> HasOverlappingShowAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null);

        Task AddWithSeatsAsync(Show show, List<ShowSeat> showSeats);
        Task UpdateAsync(Show show);
        Task DeleteAsync(Show show);
    }
}