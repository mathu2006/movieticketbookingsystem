namespace MovieTicketBooking.DTOs.Theatres
{
    public class TheatreResponseDto
    {
        public int TheatreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
