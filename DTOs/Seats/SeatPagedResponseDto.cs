namespace MovieTicketBooking.DTOs.Seats
{
    public class SeatPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<SeatResponseDto> Data { get; set; } = new();
    }
}
