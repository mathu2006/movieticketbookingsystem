using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<MoviePagedResponseDto> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? title);

        Task<MovieResponseDto?> GetByIdAsync(int movieId);

        Task<MovieResponseDto> CreateAsync(
            CreateMovieRequestDto request);

        Task<MovieResponseDto?> UpdateAsync(
            int movieId,
            UpdateMovieRequestDto request);

        Task<bool> DeleteAsync(int movieId);
    }
}