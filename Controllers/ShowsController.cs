
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        // GET: api/shows
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateOnly? date = null,
            [FromQuery] int? movieId = null,
            [FromQuery] int? theatreId = null)
        {
            try
            {
                var shows = await _showService.GetAllAsync(
                    pageNumber, pageSize, date, movieId, theatreId);

                return Ok(shows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/shows/search?date=2026-11-02
        [HttpGet("search")]
        public async Task<IActionResult> SearchByDate(
            [FromQuery] DateOnly date,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var shows = await _showService.GetByDateAsync(
                    date, pageNumber, pageSize);

                return Ok(shows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/shows/movie/1
        [HttpGet("movie/{movieId:int}")]
        public async Task<IActionResult> GetByMovieId(
            int movieId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var shows = await _showService.GetByMovieIdAsync(
                    movieId, pageNumber, pageSize);

                return Ok(shows);
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

        // GET: api/shows/theatre/1
        [HttpGet("theatre/{theatreId:int}")]
        public async Task<IActionResult> GetByTheatreId(
            int theatreId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var shows = await _showService.GetByTheatreIdAsync(
                    theatreId, pageNumber, pageSize);

                return Ok(shows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/shows/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var show = await _showService.GetByIdAsync(id);

                if (show == null)
                    return NotFound(new { message = "Show not found." });

                return Ok(show);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // GET ALL SEATS FOR A SHOW
        [HttpGet("{showId:int}/seats")]
        public async Task<IActionResult> GetShowSeats(int showId)
        {
            try
            {
                var seats = await _showService.GetShowSeatsAsync(showId);
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

        // GET AVAILABLE SEATS FOR A SHOW
        [HttpGet("{showId:int}/seats/available")]
        public async Task<IActionResult> GetAvailableShowSeats(int showId)
        {
            try
            {
                var seats = await _showService.GetAvailableShowSeatsAsync(showId);
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

        // POST: api/shows
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateShowRequestDto request)
        {
            try
            {
                var show = await _showService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = show.ShowId },
                    show);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "The show could not be saved because of a database conflict."
                });
            }
        }

        // PUT: api/shows/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateShowRequestDto request)
        {
            try
            {
                var show = await _showService.UpdateAsync(id, request);

                if (show == null)
                    return NotFound(new { message = "Show not found." });

                return Ok(show);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "The show could not be updated because of a database conflict."
                });
            }
        }

        // DELETE: api/shows/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _showService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Show not found." });

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
                    message = "This show cannot be deleted because bookings are associated with it."
                });
            }
        }
    }
}

