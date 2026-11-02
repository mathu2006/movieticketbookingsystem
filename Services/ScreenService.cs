
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _screenRepository;

        public ScreenService(IScreenRepository screenRepository)
        {
            _screenRepository = screenRepository;
        }

        // GET ALL SCREENS WITH PAGINATION
        public async Task<ScreenPagedResponseDto> GetAllAsync(
            int? theatreId, int pageNumber, int pageSize)
        {
            ValidatePagination(pageNumber, pageSize);

            if (theatreId.HasValue)
            {
                ValidateId(theatreId.Value, "Theatre");
            }

            var screens = await _screenRepository.GetAllAsync(
                theatreId, pageNumber, pageSize);

            var totalCount =
                await _screenRepository.GetTotalCountAsync(theatreId);

            return CreatePagedResponse(
                screens, pageNumber, pageSize, totalCount);
        }

        // GET SCREEN BY ID
        public async Task<ScreenResponseDto?> GetByIdAsync(int screenId)
        {
            ValidateId(screenId, "Screen");

            var screen = await _screenRepository.GetByIdAsync(screenId);

            return screen == null ? null : MapToResponse(screen);
        }

        // GET SCREENS BY THEATRE ID WITH PAGINATION
        public async Task<ScreenPagedResponseDto> GetByTheatreIdAsync(
            int theatreId, int pageNumber, int pageSize)
        {
            ValidateId(theatreId, "Theatre");
            ValidatePagination(pageNumber, pageSize);

            if (!await _screenRepository.TheatreExistsAsync(theatreId))
            {
                throw new KeyNotFoundException("Theatre not found.");
            }

            var screens = await _screenRepository.GetByTheatreIdAsync(
                theatreId, pageNumber, pageSize);

            var totalCount =
                await _screenRepository.GetTotalCountByTheatreIdAsync(
                    theatreId);

            return CreatePagedResponse(
                screens, pageNumber, pageSize, totalCount);
        }

        // CREATE SCREEN
        public async Task<ScreenResponseDto> CreateAsync(
            CreateScreenRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentException(
                    "Screen details are required.");
            }

            ValidateScreen(request.TheatreId, request.ScreenName);

            if (!await _screenRepository.TheatreExistsAsync(
                request.TheatreId))
            {
                throw new KeyNotFoundException("Theatre not found.");
            }

            var screen = new Screen
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName.Trim()
            };

            await _screenRepository.AddAsync(screen);

            return MapToResponse(screen);
        }

        // UPDATE SCREEN
        public async Task<ScreenResponseDto?> UpdateAsync(
            int screenId,
            UpdateScreenRequestDto request)
        {
            ValidateId(screenId, "Screen");

            if (request == null)
            {
                throw new ArgumentException(
                    "Screen details are required.");
            }

            ValidateScreen(request.TheatreId, request.ScreenName);

            if (!await _screenRepository.TheatreExistsAsync(
                request.TheatreId))
            {
                throw new KeyNotFoundException("Theatre not found.");
            }

            var screen = await _screenRepository.GetByIdAsync(screenId);

            if (screen == null)
            {
                return null;
            }

            screen.TheatreId = request.TheatreId;
            screen.ScreenName = request.ScreenName.Trim();

            await _screenRepository.UpdateAsync(screen);

            return MapToResponse(screen);
        }

        // DELETE SCREEN
        public async Task<bool> DeleteAsync(int screenId)
        {
            ValidateId(screenId, "Screen");

            var screen = await _screenRepository.GetByIdAsync(screenId);

            if (screen == null)
            {
                return false;
            }

            await _screenRepository.DeleteAsync(screen);

            return true;
        }

        // PAGINATION VALIDATION
        private static void ValidatePagination(
            int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentException(
                    "Page number must be greater than zero.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                throw new ArgumentException(
                    "Page size must be between 1 and 100.");
            }

            if ((long)(pageNumber - 1) * pageSize > int.MaxValue)
            {
                throw new ArgumentException(
                    "Page number is too large.");
            }
        }

        // CREATE PAGINATED RESPONSE
        private static ScreenPagedResponseDto CreatePagedResponse(
            List<Screen> screens,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            return new ScreenPagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                Data = screens.Select(MapToResponse).ToList()
            };
        }

        // ID VALIDATION
        private static void ValidateId(int id, string entityName)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    $"{entityName} ID must be greater than zero.");
            }
        }

        // SCREEN VALIDATION
        private static void ValidateScreen(
            int theatreId, string screenName)
        {
            ValidateId(theatreId, "Theatre");

            if (string.IsNullOrWhiteSpace(screenName))
            {
                throw new ArgumentException(
                    "Screen name is required.");
            }

            if (screenName.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Screen name cannot exceed 50 characters.");
            }
        }

        // DTO MAPPING
        private static ScreenResponseDto MapToResponse(Screen screen)
        {
            return new ScreenResponseDto
            {
                ScreenId = screen.ScreenId,
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName
            };
        }
    }
}
