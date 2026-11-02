using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // GET: api/seats?pageNumber=1&pageSize=10
        // GET: api/seats?screenId=2&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? screenId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var seats = await _seatService.GetAllAsync(
                    screenId, pageNumber, pageSize);

                return Ok(seats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/seats/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var seat = await _seatService.GetByIdAsync(id);

                if (seat == null)
                {
                    return NotFound(new { message = "Seat not found." });
                }

                return Ok(seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/seats/screen/2?pageNumber=1&pageSize=10
        [HttpGet("screen/{screenId:int}")]
        public async Task<IActionResult> GetByScreenId(
            int screenId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var seats = await _seatService.GetByScreenIdAsync(
                    screenId, pageNumber, pageSize);

                return Ok(seats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/seats
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateSeatRequestDto request)
        {
            try
            {
                var seat = await _seatService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = seat.SeatId },
                    seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/seats/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(
            [FromBody] BulkCreateSeatsRequestDto request)
        {
            try
            {
                var seats = await _seatService.BulkCreateAsync(request);

                return StatusCode(
                    StatusCodes.Status201Created,
                    seats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: api/seats/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateSeatRequestDto request)
        {
            try
            {
                var seat = await _seatService.UpdateAsync(id, request);

                if (seat == null)
                {
                    return NotFound(new { message = "Seat not found." });
                }

                return Ok(seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/seats/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _seatService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Seat not found." });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "This seat cannot be deleted because it is being used by a show or booking."
                });
            }
        }
    }
}