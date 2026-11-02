namespace MovieTicketBooking.DTOs.Bookings
{
    public class BookingPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public List<BookingResponseDto> Data { get; set; } = new();
    }
}
