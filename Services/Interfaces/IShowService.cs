using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IShowService
    {
        Task<ShowPagedResponseDto> GetAllAsync(
            int pageNumber, int pageSize,
            DateOnly? date, int? movieId, int? theatreId);

        Task<ShowResponseDto?> GetByIdAsync(int showId);

        Task<List<ShowSeatResponseDto>> GetShowSeatsAsync(int showId);

        Task<List<ShowSeatResponseDto>> GetAvailableShowSeatsAsync(int showId);

        Task<ShowPagedResponseDto> GetByMovieIdAsync(
            int movieId, int pageNumber, int pageSize);

        Task<ShowPagedResponseDto> GetByTheatreIdAsync(
            int theatreId, int pageNumber, int pageSize);

        Task<ShowPagedResponseDto> GetByDateAsync(
            DateOnly date, int pageNumber, int pageSize);

        Task<ShowResponseDto> CreateAsync(CreateShowRequestDto request);

        Task<ShowResponseDto?> UpdateAsync(
            int showId, UpdateShowRequestDto request);

        Task<bool> DeleteAsync(int showId);
    }
}