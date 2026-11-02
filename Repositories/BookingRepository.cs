using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET ALL BOOKINGS WITH PAGINATION
        public async Task<List<Booking>> GetAllAsync(
            int pageNumber,
            int pageSize)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(booking => booking.BookingSeats)
                    .ThenInclude(bookingSeat => bookingSeat.Seat)
                .OrderBy(booking => booking.BookingId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // GET TOTAL BOOKING COUNT
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Bookings.CountAsync();
        }

        // GET SHOW WITH ITS SEATS
        public async Task<Show?> GetShowWithSeatsAsync(int showId)
        {
            return await _context.Shows
                .Include(show => show.ShowSeats)
                    .ThenInclude(showSeat => showSeat.Seat)
                .FirstOrDefaultAsync(show => show.ShowId == showId);
        }

        // GET BOOKING BY ID
        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(booking => booking.BookingSeats)
                    .ThenInclude(bookingSeat => bookingSeat.Seat)
                .FirstOrDefaultAsync(booking => booking.BookingId == bookingId);
        }

        // CREATE BOOKING WITH TRANSACTION
        public async Task CreateBookingAsync(
            Booking booking,
            List<ShowSeat> selectedShowSeats)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var showSeat in selectedShowSeats)
                {
                    showSeat.Status = "Booked";
                }

                await _context.Bookings.AddAsync(booking);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // CANCEL BOOKING WITH TRANSACTION
        public async Task CancelBookingAsync(
            Booking booking,
            List<ShowSeat> selectedShowSeats)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                booking.Status = "Cancelled";

                foreach (var showSeat in selectedShowSeats)
                {
                    showSeat.Status = "Available";
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}