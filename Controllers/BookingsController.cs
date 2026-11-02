using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/bookings?pageNumber=1&pageSize=10
        [HttpGet]
        [ProducesResponseType(
            typeof(BookingPagedResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingPagedResponseDto>> GetAllBookings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var bookings = await _bookingService.GetAllAsync(
                    pageNumber, pageSize);

                return Ok(bookings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/bookings
        [HttpPost]
        [ProducesResponseType(
            typeof(BookingResponseDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(
            [FromBody] CreateBookingRequestDto request)
        {
            try
            {
                var booking = await _bookingService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetBookingById),
                    new { id = booking.BookingId },
                    booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: api/bookings/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(BookingResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingResponseDto>> GetBookingById(
            int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/bookings/1/cancel
        [HttpPut("{id:int}/cancel")]
        [ProducesResponseType(
            typeof(BookingResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BookingResponseDto>> CancelBooking(
            int id)
        {
            try
            {
                var booking = await _bookingService.CancelAsync(id);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message = "Booking not found."
                    });
                }

                return Ok(booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}