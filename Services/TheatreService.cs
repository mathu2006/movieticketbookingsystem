
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class TheatreService : ITheatreService
    {
        private readonly ITheatreRepository _theatreRepository;

        public TheatreService(ITheatreRepository theatreRepository)
        {
            _theatreRepository = theatreRepository;
        }

        // GET ALL THEATRES WITH PAGINATION
        public async Task<TheatrePagedResponseDto> GetAllAsync(
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

            var theatres = await _theatreRepository.GetAllAsync(
                pageNumber, pageSize);

            var totalCount =
                await _theatreRepository.GetTotalCountAsync();

            return new TheatrePagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                Data = theatres.Select(MapToResponse).ToList()
            };
        }

        // GET THEATRE BY ID
        public async Task<TheatreResponseDto?> GetByIdAsync(int theatreId)
        {
            if (theatreId <= 0)
            {
                throw new ArgumentException(
                    "Theatre ID must be greater than zero.");
            }

            var theatre = await _theatreRepository.GetByIdAsync(theatreId);

            return theatre == null ? null : MapToResponse(theatre);
        }

        // CREATE THEATRE
        public async Task<TheatreResponseDto> CreateAsync(
            CreateTheatreRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentException(
                    "Theatre request cannot be null.");
            }

            ValidateTheatre(request.Name, request.Location);

            var theatre = new Theatre
            {
                Name = request.Name.Trim(),
                Location = request.Location.Trim()
            };

            await _theatreRepository.AddAsync(theatre);

            return MapToResponse(theatre);
        }

        // UPDATE THEATRE
        public async Task<TheatreResponseDto?> UpdateAsync(
            int theatreId,
            UpdateTheatreRequestDto request)
        {
            if (theatreId <= 0)
            {
                throw new ArgumentException(
                    "Theatre ID must be greater than zero.");
            }

            if (request == null)
            {
                throw new ArgumentException(
                    "Theatre request cannot be null.");
            }

            ValidateTheatre(request.Name, request.Location);

            var theatre = await _theatreRepository.GetByIdAsync(theatreId);

            if (theatre == null)
            {
                return null;
            }

            theatre.Name = request.Name.Trim();
            theatre.Location = request.Location.Trim();

            await _theatreRepository.UpdateAsync(theatre);

            return MapToResponse(theatre);
        }

        // DELETE THEATRE
        public async Task<bool> DeleteAsync(int theatreId)
        {
            if (theatreId <= 0)
            {
                throw new ArgumentException(
                    "Theatre ID must be greater than zero.");
            }

            var theatre = await _theatreRepository.GetByIdAsync(theatreId);

            if (theatre == null)
            {
                return false;
            }

            await _theatreRepository.DeleteAsync(theatre);

            return true;
        }

        // VALIDATION
        private static void ValidateTheatre(string name, string location)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Theatre name is required.");
            }

            if (name.Trim().Length > 150)
            {
                throw new ArgumentException(
                    "Theatre name cannot exceed 150 characters.");
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException(
                    "Theatre location is required.");
            }

            if (location.Trim().Length > 200)
            {
                throw new ArgumentException(
                    "Theatre location cannot exceed 200 characters.");
            }
        }

        // DTO MAPPING
        private static TheatreResponseDto MapToResponse(Theatre theatre)
        {
            return new TheatreResponseDto
            {
                TheatreId = theatre.TheatreId,
                Name = theatre.Name,
                Location = theatre.Location
            };
        }
    }
}
