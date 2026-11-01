using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _theatreService;

        public TheatresController(ITheatreService theatreService)
        {
            _theatreService = theatreService;
        }

        // GET: api/theatres
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var theatres = await _theatreService.GetAllAsync();

            return Ok(theatres);
        }

        // GET: api/theatres/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var theatre = await _theatreService.GetByIdAsync(id);

                if (theatre == null)
                {
                    return NotFound(new
                    {
                        message = "Theatre not found."
                    });
                }

                return Ok(theatre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/theatres
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTheatreRequestDto request)
        {
            try
            {
                var theatre = await _theatreService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = theatre.TheatreId },
                    theatre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/theatres/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTheatreRequestDto request)
        {
            try
            {
                var theatre = await _theatreService.UpdateAsync(id, request);

                if (theatre == null)
                {
                    return NotFound(new
                    {
                        message = "Theatre not found."
                    });
                }

                return Ok(theatre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/theatres/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _theatreService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Theatre not found."
                    });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
