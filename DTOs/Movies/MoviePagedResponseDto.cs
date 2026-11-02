using System.Collections.Generic;

namespace MovieTicketBooking.DTOs.Movies
{
    public class MoviePagedResponseDto
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public List<MovieResponseDto> Data { get; set; } = new();
    }
}