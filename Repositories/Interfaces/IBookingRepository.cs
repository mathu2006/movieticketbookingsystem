using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync(
            int pageNumber,
            int pageSize);

        Task<int> GetTotalCountAsync();

        Task<Show?> GetShowWithSeatsAsync(int showId);

        Task<Booking?> GetByIdAsync(int bookingId);

        Task CreateBookingAsync(
            Booking booking,
            List<ShowSeat> selectedShowSeats);

        Task CancelBookingAsync(
            Booking booking,
            List<ShowSeat> selectedShowSeats);
    }
}