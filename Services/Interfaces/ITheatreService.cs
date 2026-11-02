
using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ITheatreService
    {
        // GET ALL THEATRES WITH PAGINATION
        Task<TheatrePagedResponseDto> GetAllAsync(
            int pageNumber, int pageSize);

        // GET THEATRE BY ID
        Task<TheatreResponseDto?> GetByIdAsync(int theatreId);

        // CREATE THEATRE
        Task<TheatreResponseDto> CreateAsync(
            CreateTheatreRequestDto request);

        // UPDATE THEATRE
        Task<TheatreResponseDto?> UpdateAsync(
            int theatreId,
            UpdateTheatreRequestDto request);

        // DELETE THEATRE
        Task<bool> DeleteAsync(int theatreId);
    }
}
