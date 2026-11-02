using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ITheatreService
    {
        Task<List<TheatreResponseDto>> GetAllAsync();

        Task<TheatreResponseDto?> GetByIdAsync(int theatreId);

        Task<TheatreResponseDto> CreateAsync(CreateTheatreRequestDto request);

        Task<TheatreResponseDto?> UpdateAsync(
            int theatreId,
            UpdateTheatreRequestDto request);

        Task<bool> DeleteAsync(int theatreId);
    }
}