using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL SEATS WITH OPTIONAL SCREEN FILTER AND PAGINATION
        public async Task<List<Seat>> GetAllAsync(
            int? screenId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Seats
                .AsNoTracking()
                .AsQueryable();

            if (screenId.HasValue)
            {
                query = query.Where(s => s.ScreenId == screenId.Value);
            }

            return await query
                .OrderBy(s => s.SeatId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // GET TOTAL COUNT WITH OPTIONAL SCREEN FILTER
        public async Task<int> GetTotalCountAsync(int? screenId)
        {
            var query = _context.Seats.AsQueryable();

            if (screenId.HasValue)
            {
                query = query.Where(s => s.ScreenId == screenId.Value);
            }

            return await query.CountAsync();
        }

        // GET SEAT BY ID
        public async Task<Seat?> GetByIdAsync(int seatId)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(s => s.SeatId == seatId);
        }

        // GET SEATS BY SCREEN ID WITH PAGINATION
        public async Task<List<Seat>> GetByScreenIdAsync(
            int screenId,
            int pageNumber,
            int pageSize)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.ScreenId == screenId)
                .OrderBy(s => s.SeatId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // GET TOTAL SEAT COUNT BY SCREEN ID
        public async Task<int> GetTotalCountByScreenIdAsync(int screenId)
        {
            return await _context.Seats
                .CountAsync(s => s.ScreenId == screenId);
        }

        // ADD SINGLE SEAT
        public async Task AddAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
            await _context.SaveChangesAsync();
        }

        // ADD MULTIPLE SEATS
        public async Task AddRangeAsync(List<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        // UPDATE SEAT
        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }

        // DELETE SEAT
        public async Task DeleteAsync(Seat seat)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }

        // CHECK SCREEN EXISTS
        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _context.Screens
                .AnyAsync(s => s.ScreenId == screenId);
        }
    }
}