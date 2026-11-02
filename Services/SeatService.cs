using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        // GET ALL SEATS WITH PAGINATION
        public async Task<SeatPagedResponseDto> GetAllAsync(
            int? screenId,
            int pageNumber,
            int pageSize)
        {
            ValidatePagination(pageNumber, pageSize);

            if (screenId.HasValue && screenId.Value <= 0)
            {
                throw new ArgumentException(
                    "Screen ID must be greater than zero.");
            }

            var seats = await _seatRepository.GetAllAsync(
                screenId, pageNumber, pageSize);

            var totalCount =
                await _seatRepository.GetTotalCountAsync(screenId);

            return CreatePagedResponse(
                seats, pageNumber, pageSize, totalCount);
        }

        // GET SEAT BY ID
        public async Task<SeatResponseDto?> GetByIdAsync(int seatId)
        {
            ValidateSeatId(seatId);

            var seat = await _seatRepository.GetByIdAsync(seatId);

            return seat == null ? null : MapToResponse(seat);
        }

        // GET SEATS BY SCREEN ID WITH PAGINATION
        public async Task<SeatPagedResponseDto> GetByScreenIdAsync(
            int screenId,
            int pageNumber,
            int pageSize)
        {
            ValidateSeatId(screenId);
            ValidatePagination(pageNumber, pageSize);

            if (!await _seatRepository.ScreenExistsAsync(screenId))
            {
                throw new KeyNotFoundException("Screen not found.");
            }

            var seats = await _seatRepository.GetByScreenIdAsync(
                screenId, pageNumber, pageSize);

            var totalCount =
                await _seatRepository.GetTotalCountByScreenIdAsync(screenId);

            return CreatePagedResponse(
                seats, pageNumber, pageSize, totalCount);
        }

        // CREATE SINGLE SEAT
        public async Task<SeatResponseDto> CreateAsync(
            CreateSeatRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentException(
                    "Seat request cannot be null.");
            }

            ValidateSeatDetails(
                request.ScreenId,
                request.RowName,
                request.SeatNumber,
                request.SeatType);

            if (!await _seatRepository.ScreenExistsAsync(request.ScreenId))
            {
                throw new KeyNotFoundException("Screen not found.");
            }

            var seat = new Seat
            {
                ScreenId = request.ScreenId,
                RowName = request.RowName.Trim().ToUpperInvariant(),
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType.Trim()
            };

            await _seatRepository.AddAsync(seat);

            return MapToResponse(seat);
        }

        // BULK CREATE SEATS
        public async Task<List<SeatResponseDto>> BulkCreateAsync(
            BulkCreateSeatsRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentException(
                    "Bulk seat request cannot be null.");
            }

            if (request.ScreenId <= 0)
            {
                throw new ArgumentException(
                    "Screen ID must be greater than zero.");
            }

            if (request.Seats == null || request.Seats.Count == 0)
            {
                throw new ArgumentException(
                    "At least one seat is required.");
            }

            if (!await _seatRepository.ScreenExistsAsync(request.ScreenId))
            {
                throw new KeyNotFoundException("Screen not found.");
            }

            // Retrieve all existing seats for duplicate validation.
            // This is intentionally not paginated.
            var existingSeats =
                await _seatRepository.GetAllAsync(
                    request.ScreenId, 1, int.MaxValue);

            var seatKeys = existingSeats
                .Select(s => (
                    s.RowName.ToUpperInvariant(),
                    s.SeatNumber))
                .ToHashSet();

            var seatsToCreate = new List<Seat>();

            foreach (var seatRequest in request.Seats)
            {
                if (seatRequest == null)
                {
                    throw new ArgumentException(
                        "Seat details cannot be null.");
                }

                ValidateSeatDetails(
                    request.ScreenId,
                    seatRequest.RowName,
                    seatRequest.SeatNumber,
                    seatRequest.SeatType);

                string rowName =
                    seatRequest.RowName.Trim().ToUpperInvariant();

                var seatKey = (rowName, seatRequest.SeatNumber);

                if (!seatKeys.Add(seatKey))
                {
                    throw new ArgumentException(
                        $"Seat {rowName}{seatRequest.SeatNumber} already exists.");
                }

                seatsToCreate.Add(new Seat
                {
                    ScreenId = request.ScreenId,
                    RowName = rowName,
                    SeatNumber = seatRequest.SeatNumber,
                    SeatType = seatRequest.SeatType.Trim()
                });
            }

            await _seatRepository.AddRangeAsync(seatsToCreate);

            return seatsToCreate.Select(MapToResponse).ToList();
        }

        // UPDATE SEAT
        public async Task<SeatResponseDto?> UpdateAsync(
            int seatId,
            UpdateSeatRequestDto request)
        {
            ValidateSeatId(seatId);

            if (request == null)
            {
                throw new ArgumentException(
                    "Seat request cannot be null.");
            }

            ValidateSeatDetails(
                request.ScreenId,
                request.RowName,
                request.SeatNumber,
                request.SeatType);

            if (!await _seatRepository.ScreenExistsAsync(request.ScreenId))
            {
                throw new KeyNotFoundException("Screen not found.");
            }

            var seat = await _seatRepository.GetByIdAsync(seatId);

            if (seat == null)
            {
                return null;
            }

            seat.ScreenId = request.ScreenId;
            seat.RowName = request.RowName.Trim().ToUpperInvariant();
            seat.SeatNumber = request.SeatNumber;
            seat.SeatType = request.SeatType.Trim();

            await _seatRepository.UpdateAsync(seat);

            return MapToResponse(seat);
        }

        // DELETE SEAT
        public async Task<bool> DeleteAsync(int seatId)
        {
            ValidateSeatId(seatId);

            var seat = await _seatRepository.GetByIdAsync(seatId);

            if (seat == null)
            {
                return false;
            }

            await _seatRepository.DeleteAsync(seat);

            return true;
        }

        // VALIDATE PAGINATION
        private static void ValidatePagination(
            int pageNumber,
            int pageSize)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException(
                    "Page number must be greater than zero.");
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                throw new ArgumentException(
                    "Page size must be between 1 and 100.");
            }

            if ((long)(pageNumber - 1) * pageSize > int.MaxValue)
            {
                throw new ArgumentException(
                    "Requested page number is too large.");
            }
        }

        // CREATE PAGINATED RESPONSE
        private static SeatPagedResponseDto CreatePagedResponse(
            List<Seat> seats,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            return new SeatPagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                Data = seats.Select(MapToResponse).ToList()
            };
        }

        // VALIDATE SEAT ID
        private static void ValidateSeatId(int seatId)
        {
            if (seatId <= 0)
            {
                throw new ArgumentException(
                    "Seat ID must be greater than zero.");
            }
        }

        // VALIDATE SEAT DETAILS
        private static void ValidateSeatDetails(
            int screenId,
            string? rowName,
            int seatNumber,
            string? seatType)
        {
            if (screenId <= 0)
            {
                throw new ArgumentException(
                    "Screen ID must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(rowName) ||
                rowName.Trim().Length > 5)
            {
                throw new ArgumentException(
                    "Row name is required and must not exceed 5 characters.");
            }

            if (seatNumber <= 0)
            {
                throw new ArgumentException(
                    "Seat number must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(seatType) ||
                seatType.Trim().Length > 30)
            {
                throw new ArgumentException(
                    "Seat type is required and must not exceed 30 characters.");
            }
        }

        // MAP ENTITY TO RESPONSE DTO
        private static SeatResponseDto MapToResponse(Seat seat)
        {
            return new SeatResponseDto
            {
                SeatId = seat.SeatId,
                ScreenId = seat.ScreenId,
                RowName = seat.RowName,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType
            };
        }
    }
}