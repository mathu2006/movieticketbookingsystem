namespace MovieTicketBooking.DTOs.Bookings
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }

        public string BookingReference { get; set; } = string.Empty;

        public int ShowId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<string> Seats { get; set; } = new List<string>();
    }
}