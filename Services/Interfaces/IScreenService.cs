
using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IScreenService
    {
        Task<ScreenPagedResponseDto> GetAllAsync(
            int? theatreId, int pageNumber, int pageSize);

        Task<ScreenResponseDto?> GetByIdAsync(int screenId);

        Task<ScreenPagedResponseDto> GetByTheatreIdAsync(
            int theatreId, int pageNumber, int pageSize);

        Task<ScreenResponseDto> CreateAsync(
            CreateScreenRequestDto request);

        Task<ScreenResponseDto?> UpdateAsync(
            int screenId,
            UpdateScreenRequestDto request);

        Task<bool> DeleteAsync(int screenId);
    }
}
