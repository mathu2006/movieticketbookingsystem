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

        // GET ALL SCREENS
        public async Task<List<Screen>> GetAllAsync(int? theatreId)
        {
            var query = _context.Screens
                .AsNoTracking()
                .AsQueryable();

            if (theatreId.HasValue)
            {
                query = query.Where(s => s.TheatreId == theatreId.Value);
            }

            return await query
                .OrderBy(s => s.ScreenId)
                .ToListAsync();
        }

        // GET SCREEN BY ID
        public async Task<Screen?> GetByIdAsync(int screenId)
        {
            return await _context.Screens
                .FirstOrDefaultAsync(s => s.ScreenId == screenId);
        }

        // GET SCREENS BY THEATRE ID
        public async Task<List<Screen>> GetByTheatreIdAsync(int theatreId)
        {
            return await _context.Screens
                .AsNoTracking()
                .Where(s => s.TheatreId == theatreId)
                .OrderBy(s => s.ScreenId)
                .ToListAsync();
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
