using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<SeatPagedResponseDto> GetAllAsync(
            int? screenId,
            int pageNumber,
            int pageSize);

        Task<SeatResponseDto?> GetByIdAsync(int seatId);

        Task<SeatPagedResponseDto> GetByScreenIdAsync(
            int screenId,
            int pageNumber,
            int pageSize);

        Task<SeatResponseDto> CreateAsync(CreateSeatRequestDto request);

        Task<SeatResponseDto?> UpdateAsync(
            int seatId,
            UpdateSeatRequestDto request);

        Task<List<SeatResponseDto>> BulkCreateAsync(
            BulkCreateSeatsRequestDto request);

        Task<bool> DeleteAsync(int seatId);
    }
}