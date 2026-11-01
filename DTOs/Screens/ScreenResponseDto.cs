namespace MovieTicketBooking.DTOs.Screens
{
    public class ScreenResponseDto
    {
        public int ScreenId { get; set; }
        public int TheatreId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
    }
}
