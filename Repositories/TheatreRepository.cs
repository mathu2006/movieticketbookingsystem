using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class TheatreRepository : ITheatreRepository
    {
        private readonly ApplicationDbContext _context;

        public TheatreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Theatre>> GetAllAsync()
        {
            return await _context.Theatres
                .AsNoTracking()
                .OrderBy(t => t.TheatreId)
                .ToListAsync();
        }

        public async Task<Theatre?> GetByIdAsync(int theatreId)
        {
            return await _context.Theatres
                .FirstOrDefaultAsync(t => t.TheatreId == theatreId);
        }

        public async Task AddAsync(Theatre theatre)
        {
            await _context.Theatres.AddAsync(theatre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Theatre theatre)
        {
            _context.Theatres.Update(theatre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Theatre theatre)
        {
            _context.Theatres.Remove(theatre);
            await _context.SaveChangesAsync();
        }
    }
}
