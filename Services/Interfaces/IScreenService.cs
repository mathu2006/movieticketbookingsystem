using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IScreenService
    {
        Task<List<ScreenResponseDto>> GetAllAsync(int? theatreId);

        Task<ScreenResponseDto?> GetByIdAsync(int screenId);

        Task<List<ScreenResponseDto>> GetByTheatreIdAsync(int theatreId);

        Task<ScreenResponseDto> CreateAsync(CreateScreenRequestDto request);

        Task<ScreenResponseDto?> UpdateAsync(
            int screenId,
            UpdateScreenRequestDto request);

        Task<bool> DeleteAsync(int screenId);
    }
}