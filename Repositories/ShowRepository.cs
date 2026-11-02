using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL SHOWS WITH FILTERS AND PAGINATION
        public async Task<List<Show>> GetAllAsync(
            int skip, int take,
            DateOnly? date, int? movieId, int? theatreId)
        {
            IQueryable<Show> query = _context.Shows.AsNoTracking();

            if (date.HasValue)
            {
                DateTime start = date.Value.ToDateTime(TimeOnly.MinValue);
                DateTime end = start.AddDays(1);

                query = query.Where(s =>
                    s.StartTime >= start && s.StartTime < end);
            }

            if (movieId.HasValue)
                query = query.Where(s => s.MovieId == movieId.Value);

            if (theatreId.HasValue)
            {
                query = query.Where(s =>
                    s.Screen != null &&
                    s.Screen.TheatreId == theatreId.Value);
            }

            return await query
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.ShowId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetAllCountAsync(
            DateOnly? date, int? movieId, int? theatreId)
        {
            IQueryable<Show> query = _context.Shows.AsNoTracking();

            if (date.HasValue)
            {
                DateTime start = date.Value.ToDateTime(TimeOnly.MinValue);
                DateTime end = start.AddDays(1);

                query = query.Where(s =>
                    s.StartTime >= start && s.StartTime < end);
            }

            if (movieId.HasValue)
                query = query.Where(s => s.MovieId == movieId.Value);

            if (theatreId.HasValue)
            {
                query = query.Where(s =>
                    s.Screen != null &&
                    s.Screen.TheatreId == theatreId.Value);
            }

            return await query.CountAsync();
        }

        // GET SHOW BY ID
        public async Task<Show?> GetByIdAsync(int showId)
        {
            return await _context.Shows
                .FirstOrDefaultAsync(s => s.ShowId == showId);
        }

        // GET SHOWS BY MOVIE
        public async Task<List<Show>> GetByMovieIdAsync(
            int movieId, int skip, int take)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s => s.MovieId == movieId)
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.ShowId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetByMovieCountAsync(int movieId)
        {
            return await _context.Shows
                .CountAsync(s => s.MovieId == movieId);
        }

        // GET SHOWS BY THEATRE
        public async Task<List<Show>> GetByTheatreIdAsync(
            int theatreId, int skip, int take)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s =>
                    s.Screen != null &&
                    s.Screen.TheatreId == theatreId)
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.ShowId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetByTheatreCountAsync(int theatreId)
        {
            return await _context.Shows
                .CountAsync(s =>
                    s.Screen != null &&
                    s.Screen.TheatreId == theatreId);
        }

        // GET SHOWS BY DATE
        public async Task<List<Show>> GetByDateAsync(
            DateOnly date, int skip, int take)
        {
            DateTime start = date.ToDateTime(TimeOnly.MinValue);
            DateTime end = start.AddDays(1);

            return await _context.Shows
                .AsNoTracking()
                .Where(s =>
                    s.StartTime >= start &&
                    s.StartTime < end)
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.ShowId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetByDateCountAsync(DateOnly date)
        {
            DateTime start = date.ToDateTime(TimeOnly.MinValue);
            DateTime end = start.AddDays(1);

            return await _context.Shows
                .CountAsync(s =>
                    s.StartTime >= start &&
                    s.StartTime < end);
        }

        // CHECK MOVIE EXISTS
        public async Task<bool> MovieExistsAsync(int movieId)
        {
            return await _context.Movies
                .AnyAsync(m => m.MovieId == movieId);
        }

        // CHECK SCREEN EXISTS
        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _context.Screens
                .AnyAsync(s => s.ScreenId == screenId);
        }

        // GET ALL SEATS FOR A SHOW
        public async Task<List<ShowSeat>> GetShowSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .AsNoTracking()
                .Include(ss => ss.Seat)
                .Include(ss => ss.Show)
                .Where(ss => ss.ShowId == showId)
                .OrderBy(ss => ss.Seat!.RowName)
                .ThenBy(ss => ss.Seat!.SeatNumber)
                .ToListAsync();
        }

        // GET AVAILABLE SEATS FOR A SHOW
        public async Task<List<ShowSeat>> GetAvailableShowSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .AsNoTracking()
                .Include(ss => ss.Seat)
                .Include(ss => ss.Show)
                .Where(ss => ss.ShowId == showId &&
                             ss.Status == "Available")
                .OrderBy(ss => ss.Seat!.RowName)
                .ThenBy(ss => ss.Seat!.SeatNumber)
                .ToListAsync();
        }

        // GET PHYSICAL SEATS FOR A SCREEN
        public async Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.ScreenId == screenId)
                .OrderBy(s => s.RowName)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        // CHECK OVERLAPPING SHOW SCHEDULES
        public async Task<bool> HasOverlappingShowAsync(
            int screenId,
            DateTime startTime,
            DateTime endTime,
            int? excludeShowId = null)
        {
            IQueryable<Show> query = _context.Shows
                .Where(s =>
                    s.ScreenId == screenId &&
                    s.StartTime < endTime &&
                    s.EndTime > startTime);

            if (excludeShowId.HasValue)
                query = query.Where(s =>
                    s.ShowId != excludeShowId.Value);

            return await query.AnyAsync();
        }

        // CREATE SHOW WITH SHOWSEATS
        public async Task AddWithSeatsAsync(
            Show show, List<ShowSeat> showSeats)
        {
            show.ShowSeats = showSeats;

            foreach (ShowSeat showSeat in showSeats)
                showSeat.Show = show;

            await _context.Shows.AddAsync(show);
            await _context.SaveChangesAsync();
        }

        // UPDATE SHOW
        public async Task UpdateAsync(Show show)
        {
            _context.Shows.Update(show);
            await _context.SaveChangesAsync();
        }

        // DELETE SHOW
        public async Task DeleteAsync(Show show)
        {
            var showSeats = await _context.ShowSeats
                .Where(ss => ss.ShowId == show.ShowId)
                .ToListAsync();

            _context.ShowSeats.RemoveRange(showSeats);
            _context.Shows.Remove(show);

            await _context.SaveChangesAsync();
        }
    }
}