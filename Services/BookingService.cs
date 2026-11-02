using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // GET ALL BOOKINGS WITH PAGINATION
        public async Task<BookingPagedResponseDto> GetAllAsync(
            int pageNumber,
            int pageSize)
        {
            ValidatePagination(pageNumber, pageSize);

            var bookings = await _bookingRepository.GetAllAsync(
                pageNumber, pageSize);

            var totalCount =
                await _bookingRepository.GetTotalCountAsync();

            return new BookingPagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),

                Data = bookings.Select(MapToResponse).ToList()
            };
        }

        // CREATE BOOKING
        public async Task<BookingResponseDto> CreateAsync(
            CreateBookingRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.SeatIds == null || request.SeatIds.Count == 0)
                throw new ArgumentException(
                    "Please select at least one seat.");

            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
                throw new ArgumentException(
                    "Duplicate seat selection is not allowed.");

            var show = await _bookingRepository.GetShowWithSeatsAsync(
                request.ShowId);

            if (show == null)
                throw new ArgumentException("Show not found.");

            var selectedShowSeats = show.ShowSeats
                .Where(seat => request.SeatIds.Contains(seat.SeatId))
                .ToList();

            if (selectedShowSeats.Count != request.SeatIds.Count)
                throw new ArgumentException(
                    "One or more selected seats are invalid for this show.");

            if (selectedShowSeats.Any(seat => seat.Status != "Available"))
                throw new InvalidOperationException(
                    "One or more selected seats are already booked.");

            var booking = new Booking
            {
                BookingReference = "MOV" +
                    Guid.NewGuid().ToString("N")[..10].ToUpperInvariant(),

                ShowId = show.ShowId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                BookingDate = DateTime.UtcNow,
                TotalAmount = show.TicketPrice * selectedShowSeats.Count,
                Status = "Confirmed",

                BookingSeats = selectedShowSeats.Select(showSeat =>
                    new BookingSeat
                    {
                        SeatId = showSeat.SeatId,
                        Price = show.TicketPrice,
                        Seat = showSeat.Seat
                    }).ToList()
            };

            await _bookingRepository.CreateBookingAsync(
                booking, selectedShowSeats);

            return MapToResponse(booking);
        }

        // GET BOOKING BY ID
        public async Task<BookingResponseDto?> GetByIdAsync(int bookingId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null)
                return null;

            return MapToResponse(booking);
        }

        // CANCEL BOOKING
        public async Task<BookingResponseDto?> CancelAsync(int bookingId)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Invalid booking ID.");

            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null)
                return null;

            if (booking.Status == "Cancelled")
                return MapToResponse(booking);

            if (booking.Status != "Confirmed")
                throw new InvalidOperationException(
                    "Only confirmed bookings can be cancelled.");

            var show = await _bookingRepository.GetShowWithSeatsAsync(
                booking.ShowId);

            if (show == null)
                throw new InvalidOperationException(
                    "The associated show was not found.");

            var bookedSeatIds = booking.BookingSeats
                .Select(seat => seat.SeatId)
                .ToHashSet();

            var selectedShowSeats = show.ShowSeats
                .Where(seat => bookedSeatIds.Contains(seat.SeatId))
                .ToList();

            if (selectedShowSeats.Count != bookedSeatIds.Count)
                throw new InvalidOperationException(
                    "Some booking seat records could not be found.");

            await _bookingRepository.CancelBookingAsync(
                booking, selectedShowSeats);

            return MapToResponse(booking);
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

        // MAP ENTITY TO RESPONSE DTO
        private static BookingResponseDto MapToResponse(Booking booking)
        {
            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingReference,
                ShowId = booking.ShowId,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,

                Seats = booking.BookingSeats
                    .Where(seat => seat.Seat != null)
                    .Select(seat =>
                        $"{seat.Seat!.RowName}{seat.Seat.SeatNumber}")
                    .ToList()
            };
        }
    }
}