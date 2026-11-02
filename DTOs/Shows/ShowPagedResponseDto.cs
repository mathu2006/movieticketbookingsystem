using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.DTOs.Shows
{
    public class ShowPagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<ShowResponseDto> Data { get; set; } = new();
    }
}