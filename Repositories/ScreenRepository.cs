
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL SCREENS WITH PAGINATION
        public async Task<List<Screen>> GetAllAsync(
            int? theatreId, int pageNumber, int pageSize)
        {
            var query = _context.Screens
                .AsNoTracking()
                .AsQueryable();

            if (theatreId.HasValue)
            {
                query = query.Where(
                    s => s.TheatreId == theatreId.Value);
            }

            return await query
                .OrderBy(s => s.ScreenId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // GET TOTAL SCREEN COUNT
        public async Task<int> GetTotalCountAsync(int? theatreId)
        {
            var query = _context.Screens.AsQueryable();

            if (theatreId.HasValue)
            {
                query = query.Where(
                    s => s.TheatreId == theatreId.Value);
            }

            return await query.CountAsync();
        }

        // GET SCREEN BY ID
        public async Task<Screen?> GetByIdAsync(int screenId)
        {
            return await _context.Screens
                .FirstOrDefaultAsync(
                    s => s.ScreenId == screenId);
        }

        // GET SCREENS BY THEATRE ID WITH PAGINATION
        public async Task<List<Screen>> GetByTheatreIdAsync(
            int theatreId, int pageNumber, int pageSize)
        {
            return await _context.Screens
                .AsNoTracking()
                .Where(s => s.TheatreId == theatreId)
                .OrderBy(s => s.ScreenId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // GET TOTAL SCREENS FOR A THEATRE
        public async Task<int> GetTotalCountByTheatreIdAsync(
            int theatreId)
        {
            return await _context.Screens
                .CountAsync(s => s.TheatreId == theatreId);
        }

        // CREATE SCREEN
        public async Task AddAsync(Screen screen)
        {
            await _context.Screens.AddAsync(screen);
            await _context.SaveChangesAsync();
        }

        // UPDATE SCREEN
        public async Task UpdateAsync(Screen screen)
        {
            _context.Screens.Update(screen);
            await _context.SaveChangesAsync();
        }

        // DELETE SCREEN
        public async Task DeleteAsync(Screen screen)
        {
            _context.Screens.Remove(screen);
            await _context.SaveChangesAsync();
        }

        // CHECK THEATRE EXISTS
        public async Task<bool> TheatreExistsAsync(int theatreId)
        {
            return await _context.Theatres
                .AnyAsync(t => t.TheatreId == theatreId);
        }
    }
}
