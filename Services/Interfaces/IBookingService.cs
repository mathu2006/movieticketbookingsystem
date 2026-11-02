using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingPagedResponseDto> GetAllAsync(
            int pageNumber,
            int pageSize);

        Task<BookingResponseDto> CreateAsync(
            CreateBookingRequestDto request);

        Task<BookingResponseDto?> GetByIdAsync(int bookingId);

        Task<BookingResponseDto?> CancelAsync(int bookingId);
    }
}