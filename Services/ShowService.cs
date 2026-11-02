using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;

        public ShowService(IShowRepository showRepository)
        {
            _showRepository = showRepository;
        }

        // GET ALL SHOWS WITH PAGINATION AND FILTERS
        public async Task<ShowPagedResponseDto> GetAllAsync(
            int pageNumber,
            int pageSize,
            DateOnly? date,
            int? movieId,
            int? theatreId)
        {
            ValidatePagination(pageNumber, pageSize);

            if (movieId.HasValue && movieId.Value <= 0)
                throw new ArgumentException(
                    "Movie ID must be greater than zero.");

            if (theatreId.HasValue && theatreId.Value <= 0)
                throw new ArgumentException(
                    "Theatre ID must be greater than zero.");

            int skip = (int)((long)(pageNumber - 1) * pageSize);

            var shows = await _showRepository.GetAllAsync(
                skip, pageSize, date, movieId, theatreId);

            int totalCount = await _showRepository.GetAllCountAsync(
                date, movieId, theatreId);

            return CreatePagedResponse(
                shows, pageNumber, pageSize, totalCount);
        }

        // GET SHOW BY ID
        public async Task<ShowResponseDto?> GetByIdAsync(int showId)
        {
            ValidateId(showId, "Show");

            var show = await _showRepository.GetByIdAsync(showId);

            return show == null ? null : MapToResponse(show);
        }

        // GET ALL SEATS FOR A SHOW
        public async Task<List<ShowSeatResponseDto>> GetShowSeatsAsync(int showId)
        {
            ValidateId(showId, "Show");

            var show = await _showRepository.GetByIdAsync(showId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            var showSeats = await _showRepository.GetShowSeatsAsync(showId);

            return showSeats.Select(ss => new ShowSeatResponseDto
            {
                SeatId = ss.SeatId,
                RowName = ss.Seat!.RowName,
                SeatNumber = ss.Seat.SeatNumber,
                SeatLabel = $"{ss.Seat.RowName}{ss.Seat.SeatNumber}",
                SeatType = ss.Seat.SeatType,
                Price = ss.Show!.TicketPrice,
                Status = ss.Status
            }).ToList();
        }

        // GET AVAILABLE SEATS FOR A SHOW
        public async Task<List<ShowSeatResponseDto>> GetAvailableShowSeatsAsync(int showId)
        {
            ValidateId(showId, "Show");

            var show = await _showRepository.GetByIdAsync(showId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            var showSeats =
                await _showRepository.GetAvailableShowSeatsAsync(showId);

            return showSeats.Select(ss => new ShowSeatResponseDto
            {
                SeatId = ss.SeatId,
                RowName = ss.Seat!.RowName,
                SeatNumber = ss.Seat.SeatNumber,
                SeatLabel = $"{ss.Seat.RowName}{ss.Seat.SeatNumber}",
                SeatType = ss.Seat.SeatType,
                Price = ss.Show!.TicketPrice,
                Status = ss.Status
            }).ToList();
        }

        // GET SHOWS BY MOVIE
        public async Task<ShowPagedResponseDto> GetByMovieIdAsync(
            int movieId,
            int pageNumber,
            int pageSize)
        {
            ValidateId(movieId, "Movie");
            ValidatePagination(pageNumber, pageSize);

            if (!await _showRepository.MovieExistsAsync(movieId))
                throw new KeyNotFoundException("Movie not found.");

            int skip = (int)((long)(pageNumber - 1) * pageSize);

            var shows = await _showRepository.GetByMovieIdAsync(
                movieId, skip, pageSize);

            int totalCount =
                await _showRepository.GetByMovieCountAsync(movieId);

            return CreatePagedResponse(
                shows, pageNumber, pageSize, totalCount);
        }

        // GET SHOWS BY THEATRE
        public async Task<ShowPagedResponseDto> GetByTheatreIdAsync(
            int theatreId,
            int pageNumber,
            int pageSize)
        {
            ValidateId(theatreId, "Theatre");
            ValidatePagination(pageNumber, pageSize);

            int skip = (int)((long)(pageNumber - 1) * pageSize);

            var shows = await _showRepository.GetByTheatreIdAsync(
                theatreId, skip, pageSize);

            int totalCount =
                await _showRepository.GetByTheatreCountAsync(theatreId);

            return CreatePagedResponse(
                shows, pageNumber, pageSize, totalCount);
        }

        // GET SHOWS BY DATE
        public async Task<ShowPagedResponseDto> GetByDateAsync(
            DateOnly date,
            int pageNumber,
            int pageSize)
        {
            ValidatePagination(pageNumber, pageSize);

            int skip = (int)((long)(pageNumber - 1) * pageSize);

            var shows = await _showRepository.GetByDateAsync(
                date, skip, pageSize);

            int totalCount =
                await _showRepository.GetByDateCountAsync(date);

            return CreatePagedResponse(
                shows, pageNumber, pageSize, totalCount);
        }

        // CREATE SHOW
        public async Task<ShowResponseDto> CreateAsync(
            CreateShowRequestDto request)
        {
            if (request == null)
                throw new ArgumentException("Show details are required.");

            ValidateShowDetails(
                request.MovieId,
                request.ScreenId,
                request.StartTime,
                request.EndTime,
                request.TicketPrice);

            if (!await _showRepository.MovieExistsAsync(request.MovieId))
                throw new KeyNotFoundException("Movie not found.");

            if (!await _showRepository.ScreenExistsAsync(request.ScreenId))
                throw new KeyNotFoundException("Screen not found.");

            bool hasOverlap = await _showRepository.HasOverlappingShowAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime);

            if (hasOverlap)
                throw new ArgumentException(
                    "Another show is already scheduled on this screen during that time.");

            var physicalSeats =
                await _showRepository.GetSeatsByScreenIdAsync(request.ScreenId);

            if (physicalSeats.Count == 0)
                throw new ArgumentException(
                    "No physical seats are configured for this screen.");

            var show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TicketPrice = request.TicketPrice
            };

            var showSeats = physicalSeats.Select(seat => new ShowSeat
            {
                SeatId = seat.SeatId,
                Status = "Available"
            }).ToList();

            await _showRepository.AddWithSeatsAsync(show, showSeats);

            return MapToResponse(show);
        }

        // UPDATE SHOW
        public async Task<ShowResponseDto?> UpdateAsync(
            int showId,
            UpdateShowRequestDto request)
        {
            ValidateId(showId, "Show");

            if (request == null)
                throw new ArgumentException("Show details are required.");

            var show = await _showRepository.GetByIdAsync(showId);

            if (show == null)
                return null;

            ValidateShowDetails(
                request.MovieId,
                request.ScreenId,
                request.StartTime,
                request.EndTime,
                request.TicketPrice);

            if (!await _showRepository.MovieExistsAsync(request.MovieId))
                throw new KeyNotFoundException("Movie not found.");

            if (!await _showRepository.ScreenExistsAsync(request.ScreenId))
                throw new KeyNotFoundException("Screen not found.");

            if (request.ScreenId != show.ScreenId)
                throw new ArgumentException(
                    "Changing the screen of an existing show is not supported.");

            bool hasOverlap = await _showRepository.HasOverlappingShowAsync(
                request.ScreenId,
                request.StartTime,
                request.EndTime,
                showId);

            if (hasOverlap)
                throw new ArgumentException(
                    "Another show is already scheduled on this screen during that time.");

            show.MovieId = request.MovieId;
            show.StartTime = request.StartTime;
            show.EndTime = request.EndTime;
            show.TicketPrice = request.TicketPrice;

            await _showRepository.UpdateAsync(show);

            return MapToResponse(show);
        }

        // DELETE SHOW
        public async Task<bool> DeleteAsync(int showId)
        {
            ValidateId(showId, "Show");

            var show = await _showRepository.GetByIdAsync(showId);

            if (show == null)
                return false;

            await _showRepository.DeleteAsync(show);

            return true;
        }

        // CREATE PAGINATED RESPONSE
        private static ShowPagedResponseDto CreatePagedResponse(
            List<Show> shows,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            return new ShowPagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                Data = shows.Select(MapToResponse).ToList()
            };
        }

        // VALIDATE SHOW DETAILS
        private static void ValidateShowDetails(
            int movieId,
            int screenId,
            DateTime startTime,
            DateTime endTime,
            decimal ticketPrice)
        {
            ValidateId(movieId, "Movie");
            ValidateId(screenId, "Screen");

            if (endTime <= startTime)
                throw new ArgumentException(
                    "End time must be later than start time.");

            if (ticketPrice < 0)
                throw new ArgumentException(
                    "Ticket price cannot be negative.");
        }

        // VALIDATE PAGINATION
        private static void ValidatePagination(
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException(
                    "Page number must be at least 1.");

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException(
                    "Page size must be between 1 and 100.");

            if ((long)(pageNumber - 1) * pageSize > int.MaxValue)
                throw new ArgumentException(
                    "Requested page number is too large.");
        }

        // VALIDATE ID
        private static void ValidateId(int id, string entityName)
        {
            if (id <= 0)
                throw new ArgumentException(
                    $"{entityName} ID must be greater than zero.");
        }

        // MAP SHOW ENTITY TO RESPONSE DTO
        private static ShowResponseDto MapToResponse(Show show)
        {
            return new ShowResponseDto
            {
                ShowId = show.ShowId,
                MovieId = show.MovieId,
                ScreenId = show.ScreenId,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                TicketPrice = show.TicketPrice
            };
        }
    }
}