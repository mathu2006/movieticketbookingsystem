using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        // GET: api/screens?theatreId=1
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? theatreId)
        {
            try
            {
                var screens = await _screenService.GetAllAsync(theatreId);
                return Ok(screens);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/screens/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var screen = await _screenService.GetByIdAsync(id);

                if (screen == null)
                {
                    return NotFound(new { message = "Screen not found." });
                }

                return Ok(screen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/screens/theatre/1
        [HttpGet("theatre/{theatreId:int}")]
        public async Task<IActionResult> GetByTheatreId(int theatreId)
        {
            try
            {
                var screens = await _screenService.GetByTheatreIdAsync(theatreId);
                return Ok(screens);
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

        // POST: api/screens
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateScreenRequestDto request)
        {
            try
            {
                var screen = await _screenService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = screen.ScreenId },
                    screen);
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

        // PUT: api/screens/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateScreenRequestDto request)
        {
            try
            {
                var screen = await _screenService.UpdateAsync(id, request);

                if (screen == null)
                {
                    return NotFound(new { message = "Screen not found." });
                }

                return Ok(screen);
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

        // DELETE: api/screens/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _screenService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Screen not found." });
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
