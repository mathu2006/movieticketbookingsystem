
using System.Collections.Generic;

namespace MovieTicketBooking.DTOs.Screens
{
    public class ScreenPagedResponseDto
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public List<ScreenResponseDto> Data { get; set; } = new();
    }
}
