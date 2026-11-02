using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<List<MovieResponseDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? title)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than zero.");

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100.");

            int skip = (pageNumber - 1) * pageSize;

            List<Movie> movies = await _movieRepository.GetAllAsync(
                skip,
                pageSize,
                title);

            return movies.Select(MapToResponse).ToList();
        }

        public async Task<MovieResponseDto?> GetByIdAsync(int movieId)
        {
            if (movieId <= 0)
                throw new ArgumentException("Invalid movie ID.");

            Movie? movie = await _movieRepository.GetByIdAsync(movieId);

            return movie == null ? null : MapToResponse(movie);
        }

        public async Task<MovieResponseDto> CreateAsync(
            CreateMovieRequestDto request)
        {
            ValidateMovie(
                request.Title,
                request.DurationMinutes,
                request.Language,
                request.Genre);

            Movie movie = new Movie
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                DurationMinutes = request.DurationMinutes,
                ReleaseDate = request.ReleaseDate,
                Language = request.Language.Trim(),
                Genre = request.Genre.Trim()
            };

            await _movieRepository.AddAsync(movie);

            return MapToResponse(movie);
        }

        public async Task<MovieResponseDto?> UpdateAsync(
            int movieId,
            UpdateMovieRequestDto request)
        {
            if (movieId <= 0)
                throw new ArgumentException("Invalid movie ID.");

            ValidateMovie(
                request.Title,
                request.DurationMinutes,
                request.Language,
                request.Genre);

            Movie? movie = await _movieRepository.GetByIdAsync(movieId);

            if (movie == null)
                return null;

            movie.Title = request.Title.Trim();
            movie.Description = request.Description?.Trim();
            movie.DurationMinutes = request.DurationMinutes;
            movie.ReleaseDate = request.ReleaseDate;
            movie.Language = request.Language.Trim();
            movie.Genre = request.Genre.Trim();

            await _movieRepository.UpdateAsync(movie);

            return MapToResponse(movie);
        }

        public async Task<bool> DeleteAsync(int movieId)
        {
            if (movieId <= 0)
                throw new ArgumentException("Invalid movie ID.");

            Movie? movie = await _movieRepository.GetByIdAsync(movieId);

            if (movie == null)
                return false;

            await _movieRepository.DeleteAsync(movie);

            return true;
        }

        private static void ValidateMovie(
            string title,
            int durationMinutes,
            string language,
            string genre)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Movie title is required.");

            if (title.Trim().Length > 150)
                throw new ArgumentException("Movie title cannot exceed 150 characters.");

            if (durationMinutes <= 0)
                throw new ArgumentException("Movie duration must be greater than zero.");

            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Movie language is required.");

            if (language.Trim().Length > 50)
                throw new ArgumentException("Movie language cannot exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(genre))
                throw new ArgumentException("Movie genre is required.");

            if (genre.Trim().Length > 50)
                throw new ArgumentException("Movie genre cannot exceed 50 characters.");
        }

        private static MovieResponseDto MapToResponse(Movie movie)
        {
            return new MovieResponseDto
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                Language = movie.Language,
                Genre = movie.Genre
            };
        }
    }
}
