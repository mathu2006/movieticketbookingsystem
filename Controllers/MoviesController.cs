using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // GET: api/movies
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? title = null)
        {
            try
            {
                var movies = await _movieService.GetAllAsync(
                    pageNumber,
                    pageSize,
                    title);

                return Ok(movies);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/movies/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var movie = await _movieService.GetByIdAsync(id);

                if (movie == null)
                    return NotFound(new { message = "Movie not found." });

                return Ok(movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/movies/search?title=leo
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest(new { message = "Search title is required." });

            var movies = await _movieService.GetAllAsync(1, 100, title);

            return Ok(movies);
        }

        // POST: api/movies
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieRequestDto request)
        {
            try
            {
                var movie = await _movieService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = movie.MovieId },
                    movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/movies/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateMovieRequestDto request)
        {
            try
            {
                var movie = await _movieService.UpdateAsync(id, request);

                if (movie == null)
                    return NotFound(new { message = "Movie not found." });

                return Ok(movie);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/movies/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _movieService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Movie not found." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
