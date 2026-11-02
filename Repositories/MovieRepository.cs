using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetAllAsync(
            int skip,
            int take,
            string? title)
        {
            IQueryable<Movie> query = _context.Movies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(movie =>
                    movie.Title.Contains(title));
            }

            return await query
                .OrderBy(movie => movie.MovieId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Movie?> GetByIdAsync(int movieId)
        {
            return await _context.Movies
                .FirstOrDefaultAsync(movie => movie.MovieId == movieId);
        }

        public async Task AddAsync(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int movieId)
        {
            return await _context.Movies
                .AnyAsync(movie => movie.MovieId == movieId);
        }
    }
}