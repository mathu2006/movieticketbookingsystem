
using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.DTOs.Theatres
{
    public class TheatrePagedResponseDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public List<TheatreResponseDto> Data { get; set; } = new();
    }
}

