using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IScreenRepository
    {
        Task<List<Screen>> GetAllAsync(int? theatreId);

        Task<Screen?> GetByIdAsync(int screenId);

        Task<List<Screen>> GetByTheatreIdAsync(int theatreId);

        Task AddAsync(Screen screen);

        Task UpdateAsync(Screen screen);

        Task DeleteAsync(Screen screen);

        Task<bool> TheatreExistsAsync(int theatreId);
    }
}
