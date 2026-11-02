using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<List<Seat>> GetAllAsync(
            int? screenId,
            int pageNumber,
            int pageSize);

        Task<int> GetTotalCountAsync(int? screenId);

        Task<Seat?> GetByIdAsync(int seatId);

        Task<List<Seat>> GetByScreenIdAsync(
            int screenId,
            int pageNumber,
            int pageSize);

        Task<int> GetTotalCountByScreenIdAsync(int screenId);

        Task AddAsync(Seat seat);

        Task AddRangeAsync(List<Seat> seats);

        Task UpdateAsync(Seat seat);

        Task DeleteAsync(Seat seat);

        Task<bool> ScreenExistsAsync(int screenId);
    }
}